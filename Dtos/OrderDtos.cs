using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Backend.Dtos
{
    public class OrderItemResponseDto
    {
        public int Id { get; set; }
        public int? MenuItemId { get; set; }
        public string Name { get; set; } = null!;
        public double Price { get; set; }
        public int Quantity { get; set; }
    }

    public class OrderResponseDto
    {
        public int Id { get; set; }
        public int RelatedUserId { get; set; }
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public string TypeOrPriority { get; set; } = null!;
        public string CustomerName { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public string Address { get; set; } = null!;
        public string? Note { get; set; }
        public string Status { get; set; } = null!;
        public double Subtotal { get; set; }
        public double DeliveryFee { get; set; }
        public double Total { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<OrderItemResponseDto> Lines { get; set; } = new();
    }

    public class CartLineDto
    {
        [Required(ErrorMessage = "MenuItemId is required.")]
        public int MenuItemId { get; set; }

        [Required(ErrorMessage = "Quantity is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1.")]
        public int Quantity { get; set; }
    }

    public class CreateOrderRequestDto
    {
        [Required(ErrorMessage = "Title is required.")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Title must be between 3 and 100 characters.")]
        public string Title { get; set; } = null!;

        public string? Description { get; set; }

        [Required(ErrorMessage = "TypeOrPriority is required.")]
        public string TypeOrPriority { get; set; } = "Delivery";

        [Required(ErrorMessage = "CustomerName is required.")]
        public string CustomerName { get; set; } = null!;

        [Required(ErrorMessage = "Phone is required.")]
        public string Phone { get; set; } = null!;

        [Required(ErrorMessage = "Address is required.")]
        public string Address { get; set; } = null!;

        public string? Note { get; set; }

        [Required(ErrorMessage = "RelatedUserId is required.")]
        public int RelatedUserId { get; set; }

        [Required(ErrorMessage = "At least one order line is required.")]
        public List<CartLineDto> Lines { get; set; } = new();
    }

    public class UpdateOrderRequestDto
    {
        [Required(ErrorMessage = "Address is required.")]
        public string Address { get; set; } = null!;

        public string? Note { get; set; }
    }

    public class UpdateOrderStatusRequestDto
    {
        [Required(ErrorMessage = "Status is required.")]
        public string Status { get; set; } = null!;
    }
}
