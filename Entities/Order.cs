using System;
using System.Collections.Generic;

namespace Backend.Entities
{
    public class Order
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public string? Description { get; set; }
        public required string TypeOrPriority { get; set; } // e.g. Delivery, Pickup
        public required string CustomerName { get; set; }
        public required string Phone { get; set; }
        public required string Address { get; set; }
        public string? Note { get; set; }
        public required string Status { get; set; } // e.g. Pending, Preparing, Cancelled, Completed
        public double Subtotal { get; set; }
        public double DeliveryFee { get; set; }
        public double Total { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Foreign Key to User
        public int RelatedUserId { get; set; }
        public User RelatedUser { get; set; } = null!;

        // Navigation property for order items
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}
