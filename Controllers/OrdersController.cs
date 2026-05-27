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
        public async Task<ActionResult<IEnumerable<OrderResponseDto>>> GetOrders([FromQuery] int? userId)
        {
            IQueryable<Order> query = _context.Orders
                .Include(o => o.OrderItems)
                .OrderByDescending(o => o.CreatedAt);

            if (userId.HasValue)
            {
                query = query.Where(o => o.RelatedUserId == userId.Value);
            }

            var orders = await query
                .Select(o => new OrderResponseDto
                {
                    Id = o.Id,
                    RelatedUserId = o.RelatedUserId,
                    Title = o.Title,
                    Description = o.Description,
                    TypeOrPriority = o.TypeOrPriority,
                    CustomerName = o.CustomerName,
                    Phone = o.Phone,
                    Address = o.Address,
                    Note = o.Note,
                    Status = o.Status,
                    Subtotal = o.Subtotal,
                    DeliveryFee = o.DeliveryFee,
                    Total = o.Total,
                    CreatedAt = o.CreatedAt,
                    Lines = o.OrderItems.Select(oi => new OrderItemResponseDto
                    {
                        Id = oi.Id,
                        MenuItemId = oi.MenuItemId,
                        Name = oi.Name,
                        Price = oi.Price,
                        Quantity = oi.Quantity
                    }).ToList()
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
                return BadRequest(new { Message = $"User with ID {request.RelatedUserId} does not exist." });
            }

            if (request.Lines == null || request.Lines.Count == 0)
            {
                return BadRequest(new { Message = "Cannot place an order with an empty cart." });
            }

            // Load menu items to calculate subtotal and delivery fee
            var menuItemIds = request.Lines.Select(l => l.MenuItemId).ToList();
            var menuItems = await _context.MenuItems
                .Include(m => m.Restaurant)
                .Where(m => menuItemIds.Contains(m.Id))
                .ToListAsync();

            if (menuItems.Count != menuItemIds.Distinct().Count())
            {
                return BadRequest(new { Message = "Some menu items in your cart do not exist in the database." });
            }

            double subtotal = 0;
            var deliveryFeesByRestaurant = new HashSet<double>();

            var order = new Order
            {
                Title = request.Title,
                Description = request.Description,
                TypeOrPriority = request.TypeOrPriority,
                CustomerName = request.CustomerName.Trim(),
                Phone = request.Phone.Trim(),
                Address = request.Address.Trim(),
                Note = request.Note?.Trim(),
                Status = "Pending",
                RelatedUserId = request.RelatedUserId,
                CreatedAt = DateTime.UtcNow,
                Subtotal = 0,
                DeliveryFee = 0,
                Total = 0
            };

            var orderItemsList = new List<OrderItem>();

            foreach (var line in request.Lines)
            {
                var menuItem = menuItems.First(m => m.Id == line.MenuItemId);
                
                subtotal += menuItem.Price * line.Quantity;
                deliveryFeesByRestaurant.Add(menuItem.Restaurant.DeliveryFee);

                var orderItem = new OrderItem
                {
                    MenuItemId = menuItem.Id,
                    Name = menuItem.Name,
                    Price = menuItem.Price,
                    Quantity = line.Quantity,
                    Order = order
                };
                orderItemsList.Add(orderItem);
            }

            double deliveryFee = deliveryFeesByRestaurant.Sum();
            
            order.Subtotal = subtotal;
            order.DeliveryFee = deliveryFee;
            order.Total = subtotal + deliveryFee;
            order.OrderItems = orderItemsList;

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            var response = new OrderResponseDto
            {
                Id = order.Id,
                RelatedUserId = order.RelatedUserId,
                Title = order.Title,
                Description = order.Description,
                TypeOrPriority = order.TypeOrPriority,
                CustomerName = order.CustomerName,
                Phone = order.Phone,
                Address = order.Address,
                Note = order.Note,
                Status = order.Status,
                Subtotal = order.Subtotal,
                DeliveryFee = order.DeliveryFee,
                Total = order.Total,
                CreatedAt = order.CreatedAt,
                Lines = order.OrderItems.Select(oi => new OrderItemResponseDto
                {
                    Id = oi.Id,
                    MenuItemId = oi.MenuItemId,
                    Name = oi.Name,
                    Price = oi.Price,
                    Quantity = oi.Quantity
                }).ToList()
            };

            return CreatedAtAction(nameof(GetOrders), new { userId = order.RelatedUserId }, response);
        }

        // PUT: api/orders/{id}
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(OrderResponseDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateOrder(int id, [FromBody] UpdateOrderRequestDto request)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                return NotFound(new { Message = $"Order with ID {id} not found." });
            }

            order.Address = request.Address.Trim();
            order.Note = request.Note?.Trim();

            await _context.SaveChangesAsync();

            var response = new OrderResponseDto
            {
                Id = order.Id,
                RelatedUserId = order.RelatedUserId,
                Title = order.Title,
                Description = order.Description,
                TypeOrPriority = order.TypeOrPriority,
                CustomerName = order.CustomerName,
                Phone = order.Phone,
                Address = order.Address,
                Note = order.Note,
                Status = order.Status,
                Subtotal = order.Subtotal,
                DeliveryFee = order.DeliveryFee,
                Total = order.Total,
                CreatedAt = order.CreatedAt,
                Lines = order.OrderItems.Select(oi => new OrderItemResponseDto
                {
                    Id = oi.Id,
                    MenuItemId = oi.MenuItemId,
                    Name = oi.Name,
                    Price = oi.Price,
                    Quantity = oi.Quantity
                }).ToList()
            };

            return Ok(response);
        }

        // PUT: api/orders/{id}/cancel
        [HttpPut("{id}/cancel")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> CancelOrder(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound(new { Message = $"Order with ID {id} not found." });
            }

            if (order.Status == "Cancelled")
            {
                return BadRequest(new { Message = "Order is already cancelled." });
            }

            order.Status = "Cancelled";
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Order cancelled successfully.", Status = order.Status });
        }

        // PUT: api/orders/{id}/status
        [HttpPut("{id}/status")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateOrderStatus(int id, [FromBody] UpdateOrderStatusRequestDto request)
        {
            var allowedStatuses = new[] { "Pending", "Preparing", "Completed", "Cancelled" };
            if (!allowedStatuses.Contains(request.Status))
            {
                return BadRequest(new { Message = $"Invalid status. Allowed values are: {string.Join(", ", allowedStatuses)}" });
            }

            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound(new { Message = $"Order with ID {id} not found." });
            }

            order.Status = request.Status;
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Order status updated successfully.", Status = order.Status });
        }

        // DELETE: api/orders/{id}
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
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

            return Ok(new { Message = "Order deleted successfully." });
        }
    }
}
