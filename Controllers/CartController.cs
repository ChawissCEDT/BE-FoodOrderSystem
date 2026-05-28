using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Backend.Data;
using Backend.Dtos;
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
    public class CartController : ControllerBase
    {
        private readonly FoodOrderDbContext _context;

        public CartController(FoodOrderDbContext context)
        {
            _context = context;
        }

        // GET: api/cart?userId={userId}
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<CartItemResponseDto>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<CartItemResponseDto>>> GetCart([FromQuery] int userId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();
            int currentUserId = int.Parse(userIdClaim.Value);
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            if (userRole != "Admin" && userId != currentUserId)
            {
                return Forbid();
            }

            var userExists = await _context.Users.AnyAsync(u => u.Id == userId);
            if (!userExists)
            {
                return NotFound(new { Message = $"User with ID {userId} not found." });
            }

            var cartItems = await GetCartItemsAsync(userId);
            return Ok(cartItems);
        }

        // POST: api/cart/items
        [HttpPost("items")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<CartItemResponseDto>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<CartItemResponseDto>>> AddOrUpdateCartItem([FromBody] CartItemRequestDto request)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();
            int currentUserId = int.Parse(userIdClaim.Value);
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            if (userRole != "Admin" && request.UserId != currentUserId)
            {
                return Forbid();
            }

            var userExists = await _context.Users.AnyAsync(u => u.Id == request.UserId);
            if (!userExists)
            {
                return BadRequest(new { Message = $"User with ID {request.UserId} does not exist." });
            }

            var menuItem = await _context.MenuItems.FindAsync(request.MenuItemId);
            if (menuItem == null)
            {
                return BadRequest(new { Message = $"Menu item with ID {request.MenuItemId} does not exist." });
            }

            var cartItem = await _context.CartItems
                .FirstOrDefaultAsync(ci => ci.UserId == request.UserId && ci.MenuItemId == request.MenuItemId);

            if (cartItem != null)
            {
                cartItem.Quantity += request.Quantity;
                if (cartItem.Quantity <= 0)
                {
                    _context.CartItems.Remove(cartItem);
                }
            }
            else if (request.Quantity > 0)
            {
                cartItem = new CartItem
                {
                    UserId = request.UserId,
                    MenuItemId = request.MenuItemId,
                    Quantity = request.Quantity
                };
                _context.CartItems.Add(cartItem);
            }

            await _context.SaveChangesAsync();

            var updatedCart = await GetCartItemsAsync(request.UserId);
            return Ok(updatedCart);
        }

        // POST: api/cart/merge
        [HttpPost("merge")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<CartItemResponseDto>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<CartItemResponseDto>>> MergeCart([FromBody] CartMergeRequestDto request)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();
            int currentUserId = int.Parse(userIdClaim.Value);
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            if (userRole != "Admin" && request.UserId != currentUserId)
            {
                return Forbid();
            }

            var userExists = await _context.Users.AnyAsync(u => u.Id == request.UserId);
            if (!userExists)
            {
                return BadRequest(new { Message = $"User with ID {request.UserId} does not exist." });
            }

            if (request.Items != null && request.Items.Count > 0)
            {
                // Pre-aggregate input items to avoid database composite key violations on duplicate MenuItemIds
                var aggregatedItems = request.Items
                    .GroupBy(i => i.MenuItemId)
                    .Select(g => new CartLineDto { MenuItemId = g.Key, Quantity = g.Sum(x => x.Quantity) })
                    .ToList();

                var menuItemIds = aggregatedItems.Select(i => i.MenuItemId).ToList();
                var menuItems = await _context.MenuItems
                    .Where(m => menuItemIds.Contains(m.Id))
                    .ToListAsync();

                var existingCartItems = await _context.CartItems
                    .Where(ci => ci.UserId == request.UserId)
                    .ToListAsync();

                foreach (var mergeItem in aggregatedItems)
                {
                    if (!menuItems.Any(m => m.Id == mergeItem.MenuItemId)) continue;

                    var existing = existingCartItems.FirstOrDefault(ci => ci.MenuItemId == mergeItem.MenuItemId);
                    if (existing != null)
                    {
                        existing.Quantity += mergeItem.Quantity;
                        if (existing.Quantity <= 0)
                        {
                            _context.CartItems.Remove(existing);
                        }
                    }
                    else if (mergeItem.Quantity > 0)
                    {
                        var newCartItem = new CartItem
                        {
                            UserId = request.UserId,
                            MenuItemId = mergeItem.MenuItemId,
                            Quantity = mergeItem.Quantity
                        };
                        _context.CartItems.Add(newCartItem);
                    }
                }

                await _context.SaveChangesAsync();
            }

            var updatedCart = await GetCartItemsAsync(request.UserId);
            return Ok(updatedCart);
        }

        // DELETE: api/cart/items/{menuItemId}?userId={userId}
        [HttpDelete("items/{menuItemId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<CartItemResponseDto>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<CartItemResponseDto>>> RemoveCartItem(int menuItemId, [FromQuery] int userId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();
            int currentUserId = int.Parse(userIdClaim.Value);
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            if (userRole != "Admin" && userId != currentUserId)
            {
                return Forbid();
            }

            var userExists = await _context.Users.AnyAsync(u => u.Id == userId);
            if (!userExists)
            {
                return BadRequest(new { Message = $"User with ID {userId} does not exist." });
            }

            var cartItem = await _context.CartItems
                .FirstOrDefaultAsync(ci => ci.UserId == userId && ci.MenuItemId == menuItemId);

            if (cartItem != null)
            {
                _context.CartItems.Remove(cartItem);
                await _context.SaveChangesAsync();
            }

            var updatedCart = await GetCartItemsAsync(userId);
            return Ok(updatedCart);
        }

        // DELETE: api/cart?userId={userId}
        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<CartItemResponseDto>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<CartItemResponseDto>>> ClearCart([FromQuery] int userId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();
            int currentUserId = int.Parse(userIdClaim.Value);
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            if (userRole != "Admin" && userId != currentUserId)
            {
                return Forbid();
            }

            var userExists = await _context.Users.AnyAsync(u => u.Id == userId);
            if (!userExists)
            {
                return BadRequest(new { Message = $"User with ID {userId} does not exist." });
            }

            var cartItems = await _context.CartItems.Where(ci => ci.UserId == userId).ToListAsync();
            if (cartItems.Any())
            {
                _context.CartItems.RemoveRange(cartItems);
                await _context.SaveChangesAsync();
            }

            return Ok(new List<CartItemResponseDto>());
        }

        private async Task<List<CartItemResponseDto>> GetCartItemsAsync(int userId)
        {
            return await _context.CartItems
                .Include(ci => ci.MenuItem)
                .ThenInclude(m => m.Restaurant)
                .Where(ci => ci.UserId == userId)
                .Select(ci => new CartItemResponseDto
                {
                    Item = new MenuItemResponseDto
                    {
                        Id = ci.MenuItem.Id,
                        RestaurantId = ci.MenuItem.RestaurantId,
                        Name = ci.MenuItem.Name,
                        Description = ci.MenuItem.Description,
                        Category = ci.MenuItem.Category,
                        ImageUrl = ci.MenuItem.ImageUrl,
                        Price = ci.MenuItem.Price,
                        Popular = ci.MenuItem.Popular,
                        IsAvailable = ci.MenuItem.IsAvailable
                    },
                    Restaurant = new RestaurantResponseDto
                    {
                        Id = ci.MenuItem.Restaurant.Id,
                        Name = ci.MenuItem.Restaurant.Name,
                        Cuisine = ci.MenuItem.Restaurant.Cuisine,
                        Description = ci.MenuItem.Restaurant.Description,
                        Rating = ci.MenuItem.Restaurant.Rating,
                        DeliveryTime = ci.MenuItem.Restaurant.DeliveryTime,
                        DeliveryFee = ci.MenuItem.Restaurant.DeliveryFee,
                        ImageTone = ci.MenuItem.Restaurant.ImageTone,
                        ImageUrl = ci.MenuItem.Restaurant.ImageUrl,
                        IsOpen = ci.MenuItem.Restaurant.IsOpen
                    },
                    Quantity = ci.Quantity
                })
                .ToListAsync();
        }
    }
}
