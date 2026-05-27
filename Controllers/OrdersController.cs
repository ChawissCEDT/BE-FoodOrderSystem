using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Backend.Data;
using Backend.Dtos;
using Backend.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly FoodOrderDbContext _context;

        public OrdersController(FoodOrderDbContext context)
        {
            _context = context;
        }

        // GET: api/orders
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<OrderResponseDto>))]
        public async Task<ActionResult<IEnumerable<OrderResponseDto>>> GetOrders()
        {
            var orders = await _context.Orders
                .Include(o => o.RelatedUser)
                .OrderByDescending(o => o.CreatedAt)
                .Select(o => new OrderResponseDto
                {
                    Id = o.Id,
                    Title = o.Title,
                    Description = o.Description,
                    Status = o.Status,
                    TypeOrPriority = o.TypeOrPriority,
                    CreatedAt = o.CreatedAt,
                    RelatedUserId = o.RelatedUserId,
                    RelatedUser = new UserResponseDto
                    {
                        Id = o.RelatedUser.Id,
                        FullName = o.RelatedUser.FullName,
                        Email = o.RelatedUser.Email
                    }
                })
                .ToListAsync();

            return Ok(orders);
        }

        // POST: api/orders
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(OrderResponseDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<OrderResponseDto>> CreateOrder([FromBody] CreateOrderRequestDto request)
        {
            // Verify User exists
            var user = await _context.Users.FindAsync(request.RelatedUserId);
            if (user == null)
            {
                ModelState.AddModelError("RelatedUserId", $"User with ID {request.RelatedUserId} does not exist.");
                return BadRequest(ModelState);
            }

            var order = new Order
            {
                Title = request.Title,
                Description = request.Description,
                Status = "Pending", // Default status for new orders
                TypeOrPriority = request.TypeOrPriority,
                RelatedUserId = request.RelatedUserId,
                CreatedAt = DateTime.UtcNow
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            var response = new OrderResponseDto
            {
                Id = order.Id,
                Title = order.Title,
                Description = order.Description,
                Status = order.Status,
                TypeOrPriority = order.TypeOrPriority,
                CreatedAt = order.CreatedAt,
                RelatedUserId = order.RelatedUserId,
                RelatedUser = new UserResponseDto
                {
                    Id = user.Id,
                    FullName = user.FullName,
                    Email = user.Email
                }
            };

            return CreatedAtAction(nameof(GetOrders), new { }, response);
        }

        // PUT: api/orders/{id}
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateOrder(int id, [FromBody] UpdateOrderRequestDto request)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound(new { Message = $"Order with ID {id} not found." });
            }

            // Verify User exists
            var user = await _context.Users.FindAsync(request.RelatedUserId);
            if (user == null)
            {
                ModelState.AddModelError("RelatedUserId", $"User with ID {request.RelatedUserId} does not exist.");
                return BadRequest(ModelState);
            }

            order.Title = request.Title;
            order.Description = request.Description;
            order.Status = request.Status;
            order.TypeOrPriority = request.TypeOrPriority;
            order.RelatedUserId = request.RelatedUserId;

            _context.Entry(order).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrderExists(id))
                {
                    return NotFound();
                }
                throw;
            }

            return NoContent();
        }

        // DELETE: api/orders/{id}
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound(new { Message = $"Order with ID {id} not found." });
            }

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool OrderExists(int id)
        {
            return _context.Orders.Any(e => e.Id == id);
        }
    }
}
