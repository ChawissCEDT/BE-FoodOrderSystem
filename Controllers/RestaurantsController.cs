using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Backend.Data;
using Backend.Dtos;
using Microsoft.AspNetCore.Authorization;
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
                    ImageUrl = r.ImageUrl,
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
                    ImageUrl = m.ImageUrl,
                    Price = m.Price,
                    Popular = m.Popular,
                    IsAvailable = m.IsAvailable
                })
                .ToListAsync();

            return Ok(menuItems);
        }

        // PUT: api/restaurants/{id}/toggle-status
        [HttpPut("{id}/toggle-status")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ToggleRestaurantStatus(int id)
        {
            var restaurant = await _context.Restaurants.FindAsync(id);
            if (restaurant == null)
            {
                return NotFound(new { Message = $"Restaurant with ID {id} not found." });
            }

            restaurant.IsOpen = !restaurant.IsOpen;
            await _context.SaveChangesAsync();

            return Ok(new
            {
                Message = "Restaurant status updated successfully.",
                Id = restaurant.Id,
                IsOpen = restaurant.IsOpen
            });
        }

        // POST: api/restaurants
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(RestaurantResponseDto))]
        public async Task<ActionResult<RestaurantResponseDto>> CreateRestaurant([FromBody] CreateRestaurantRequestDto request)
        {
            var restaurant = new Backend.Entities.Restaurant
            {
                Name = request.Name.Trim(),
                Cuisine = request.Cuisine.Trim(),
                Description = request.Description.Trim(),
                Rating = request.Rating,
                DeliveryTime = request.DeliveryTime.Trim(),
                DeliveryFee = request.DeliveryFee,
                ImageTone = request.ImageTone.Trim(),
                ImageUrl = request.ImageUrl.Trim(),
                IsOpen = true
            };

            _context.Restaurants.Add(restaurant);
            await _context.SaveChangesAsync();

            var response = new RestaurantResponseDto
            {
                Id = restaurant.Id,
                Name = restaurant.Name,
                Cuisine = restaurant.Cuisine,
                Description = restaurant.Description,
                Rating = restaurant.Rating,
                DeliveryTime = restaurant.DeliveryTime,
                DeliveryFee = restaurant.DeliveryFee,
                ImageTone = restaurant.ImageTone,
                ImageUrl = restaurant.ImageUrl,
                IsOpen = restaurant.IsOpen
            };

            return CreatedAtAction(nameof(GetRestaurants), new { }, response);
        }

        // PUT: api/restaurants/{id}
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(RestaurantResponseDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateRestaurant(int id, [FromBody] UpdateRestaurantRequestDto request)
        {
            var restaurant = await _context.Restaurants.FindAsync(id);
            if (restaurant == null)
            {
                return NotFound(new { Message = $"Restaurant with ID {id} not found." });
            }

            restaurant.Name = request.Name.Trim();
            restaurant.Cuisine = request.Cuisine.Trim();
            restaurant.Description = request.Description.Trim();
            restaurant.Rating = request.Rating;
            restaurant.DeliveryTime = request.DeliveryTime.Trim();
            restaurant.DeliveryFee = request.DeliveryFee;
            restaurant.ImageTone = request.ImageTone.Trim();
            restaurant.ImageUrl = request.ImageUrl.Trim();
            restaurant.IsOpen = request.IsOpen;

            await _context.SaveChangesAsync();

            var response = new RestaurantResponseDto
            {
                Id = restaurant.Id,
                Name = restaurant.Name,
                Cuisine = restaurant.Cuisine,
                Description = restaurant.Description,
                Rating = restaurant.Rating,
                DeliveryTime = restaurant.DeliveryTime,
                DeliveryFee = restaurant.DeliveryFee,
                ImageTone = restaurant.ImageTone,
                ImageUrl = restaurant.ImageUrl,
                IsOpen = restaurant.IsOpen
            };

            return Ok(response);
        }

        // DELETE: api/restaurants/{id}
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteRestaurant(int id)
        {
            var restaurant = await _context.Restaurants.FindAsync(id);
            if (restaurant == null)
            {
                return NotFound(new { Message = $"Restaurant with ID {id} not found." });
            }

            _context.Restaurants.Remove(restaurant);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Restaurant deleted successfully." });
        }

        // GET: api/restaurants/{id}/menu-admin (returns all items, including unavailable)
        [HttpGet("{id}/menu-admin")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<MenuItemResponseDto>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<MenuItemResponseDto>>> GetRestaurantMenuAdmin(int id)
        {
            var restaurantExists = await _context.Restaurants.AnyAsync(r => r.Id == id);
            if (!restaurantExists)
            {
                return NotFound(new { Message = $"Restaurant with ID {id} not found." });
            }

            var menuItems = await _context.MenuItems
                .Where(m => m.RestaurantId == id)
                .Select(m => new MenuItemResponseDto
                {
                    Id = m.Id,
                    RestaurantId = m.RestaurantId,
                    Name = m.Name,
                    Description = m.Description,
                    Category = m.Category,
                    ImageUrl = m.ImageUrl,
                    Price = m.Price,
                    Popular = m.Popular,
                    IsAvailable = m.IsAvailable
                })
                .ToListAsync();

            return Ok(menuItems);
        }

        // POST: api/restaurants/{restaurantId}/menu
        [HttpPost("{restaurantId}/menu")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(MenuItemResponseDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<MenuItemResponseDto>> CreateMenuItem(int restaurantId, [FromBody] CreateMenuItemRequestDto request)
        {
            var restaurantExists = await _context.Restaurants.AnyAsync(r => r.Id == restaurantId);
            if (!restaurantExists)
            {
                return NotFound(new { Message = $"Restaurant with ID {restaurantId} not found." });
            }

            var menuItem = new Backend.Entities.MenuItem
            {
                RestaurantId = restaurantId,
                Name = request.Name.Trim(),
                Description = request.Description.Trim(),
                Category = request.Category.Trim(),
                ImageUrl = request.ImageUrl.Trim(),
                Price = request.Price,
                Popular = request.Popular,
                IsAvailable = true
            };

            _context.MenuItems.Add(menuItem);
            await _context.SaveChangesAsync();

            var response = new MenuItemResponseDto
            {
                Id = menuItem.Id,
                RestaurantId = menuItem.RestaurantId,
                Name = menuItem.Name,
                Description = menuItem.Description,
                Category = menuItem.Category,
                ImageUrl = menuItem.ImageUrl,
                Price = menuItem.Price,
                Popular = menuItem.Popular,
                IsAvailable = menuItem.IsAvailable
            };

            return CreatedAtAction(nameof(GetRestaurantMenuAdmin), new { id = restaurantId }, response);
        }

        // PUT: api/restaurants/menu/{id}
        [HttpPut("menu/{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(MenuItemResponseDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateMenuItem(int id, [FromBody] UpdateMenuItemRequestDto request)
        {
            var menuItem = await _context.MenuItems.FindAsync(id);
            if (menuItem == null)
            {
                return NotFound(new { Message = $"Menu item with ID {id} not found." });
            }

            menuItem.Name = request.Name.Trim();
            menuItem.Description = request.Description.Trim();
            menuItem.Category = request.Category.Trim();
            menuItem.ImageUrl = request.ImageUrl.Trim();
            menuItem.Price = request.Price;
            menuItem.Popular = request.Popular;
            menuItem.IsAvailable = request.IsAvailable;

            await _context.SaveChangesAsync();

            var response = new MenuItemResponseDto
            {
                Id = menuItem.Id,
                RestaurantId = menuItem.RestaurantId,
                Name = menuItem.Name,
                Description = menuItem.Description,
                Category = menuItem.Category,
                ImageUrl = menuItem.ImageUrl,
                Price = menuItem.Price,
                Popular = menuItem.Popular,
                IsAvailable = menuItem.IsAvailable
            };

            return Ok(response);
        }

        // DELETE: api/restaurants/menu/{id}
        [HttpDelete("menu/{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteMenuItem(int id)
        {
            var menuItem = await _context.MenuItems.FindAsync(id);
            if (menuItem == null)
            {
                return NotFound(new { Message = $"Menu item with ID {id} not found." });
            }

            _context.MenuItems.Remove(menuItem);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Menu item deleted successfully." });
        }
    }
}
