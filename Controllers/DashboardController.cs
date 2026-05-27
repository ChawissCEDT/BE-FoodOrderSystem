using System;
using System.Linq;
using System.Threading.Tasks;
using Backend.Data;
using Backend.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
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
            var restaurantsCount = await _context.Restaurants.CountAsync();
            var openRestaurantsCount = await _context.Restaurants.CountAsync(r => r.IsOpen);
            var menuItemsCount = await _context.MenuItems.CountAsync();

            // Calculate active orders and revenue
            IQueryable<Order> ordersQuery = _context.Orders;
            
            if (userId.HasValue)
            {
                ordersQuery = ordersQuery.Where(o => o.RelatedUserId == userId.Value);
            }

            var activeOrdersCount = await ordersQuery.CountAsync(o => o.Status != "Cancelled");
            var totalRevenue = await ordersQuery
                .Where(o => o.Status != "Cancelled")
                .SumAsync(o => o.Total);

            var cartItemsCount = 0;
            if (userId.HasValue)
            {
                cartItemsCount = await _context.CartItems
                    .Where(ci => ci.UserId == userId.Value)
                    .SumAsync(ci => (int?)ci.Quantity) ?? 0;
            }

            var response = new
            {
                Restaurants = restaurantsCount,
                OpenRestaurants = openRestaurantsCount,
                MenuItems = menuItemsCount,
                CartItems = cartItemsCount,
                ActiveOrders = activeOrdersCount,
                Revenue = totalRevenue
            };

            return Ok(response);
        }
    }
}
