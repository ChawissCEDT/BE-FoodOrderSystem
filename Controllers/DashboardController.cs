using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Backend.Data;
using Backend.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class DashboardController : ControllerBase
    {
        private readonly FoodOrderDbContext _context;

        public DashboardController(FoodOrderDbContext context)
        {
            _context = context;
        }

        // GET: api/dashboard/summary
        [HttpGet("summary")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetSummary([FromQuery] int? userId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();
            int currentUserId = int.Parse(userIdClaim.Value);
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            int? targetUserId = userId;
            if (userRole != "Admin")
            {
                // Customers are strictly forced to view their own summary metrics
                targetUserId = currentUserId;
            }

            // Start independent DB queries concurrently
            var restaurantsCountTask = _context.Restaurants.CountAsync();
            var openRestaurantsCountTask = _context.Restaurants.CountAsync(r => r.IsOpen);
            var menuItemsCountTask = _context.MenuItems.CountAsync();

            IQueryable<Order> ordersQuery = _context.Orders;
            if (targetUserId.HasValue)
            {
                ordersQuery = ordersQuery.Where(o => o.RelatedUserId == targetUserId.Value);
            }

            var activeOrdersCountTask = ordersQuery.CountAsync(o => o.Status != "Cancelled");
            var totalRevenueTask = ordersQuery
                .Where(o => o.Status != "Cancelled")
                .SumAsync(o => o.Total);

            var cartItemsCountTask = targetUserId.HasValue
                ? _context.CartItems.Where(ci => ci.UserId == targetUserId.Value).SumAsync(ci => (int?)ci.Quantity)
                : Task.FromResult<int?>(0);

            // Await all database queries concurrently
            await Task.WhenAll(
                restaurantsCountTask,
                openRestaurantsCountTask,
                menuItemsCountTask,
                activeOrdersCountTask,
                totalRevenueTask,
                cartItemsCountTask
            );

            var response = new
            {
                Restaurants = restaurantsCountTask.Result,
                OpenRestaurants = openRestaurantsCountTask.Result,
                MenuItems = menuItemsCountTask.Result,
                CartItems = cartItemsCountTask.Result ?? 0,
                ActiveOrders = activeOrdersCountTask.Result,
                Revenue = totalRevenueTask.Result
            };

            return Ok(response);
        }
    }
}
