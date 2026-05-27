using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Backend.Data;
using Backend.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RestaurantsController : ControllerBase
    {
        private readonly FoodOrderDbContext _context;

        public RestaurantsController(FoodOrderDbContext context)
        {
            _context = context;
        }

        // GET: api/restaurants
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<RestaurantResponseDto>))]
        public async Task<ActionResult<IEnumerable<RestaurantResponseDto>>> GetRestaurants()
        {
            var restaurants = await _context.Restaurants
                .Select(r => new RestaurantResponseDto
                {
                    Id = r.Id,
                    Name = r.Name,
                    Cuisine = r.Cuisine,
                    Description = r.Description,
                    Rating = r.Rating,
                    DeliveryTime = r.DeliveryTime,
                    DeliveryFee = r.DeliveryFee,
                    ImageTone = r.ImageTone,
                    IsOpen = r.IsOpen
                })
                .ToListAsync();

            return Ok(restaurants);
        }

        // GET: api/restaurants/{id}/menu
        [HttpGet("{id}/menu")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<MenuItemResponseDto>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<MenuItemResponseDto>>> GetRestaurantMenu(int id)
        {
            var restaurantExists = await _context.Restaurants.AnyAsync(r => r.Id == id);
            if (!restaurantExists)
            {
                return NotFound(new { Message = $"Restaurant with ID {id} not found." });
            }

            var menuItems = await _context.MenuItems
                .Where(m => m.RestaurantId == id && m.IsAvailable)
                .Select(m => new MenuItemResponseDto
                {
                    Id = m.Id,
                    RestaurantId = m.RestaurantId,
                    Name = m.Name,
                    Description = m.Description,
                    Category = m.Category,
                    Price = m.Price,
                    Popular = m.Popular,
                    IsAvailable = m.IsAvailable
                })
                .ToListAsync();

            return Ok(menuItems);
        }
    }
}
