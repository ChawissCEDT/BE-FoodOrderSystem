using System;

namespace Backend.Entities
{
    public class Order
    {
        public int Id { get; set; }
        public required string Title { get; set; } // OrderName
        public string? Description { get; set; }
        public required string Status { get; set; } // e.g. Pending, Preparing, Cancelled, Completed
        public required string TypeOrPriority { get; set; } // e.g. Delivery, Pickup
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Foreign Key to User
        public int RelatedUserId { get; set; }
        public User RelatedUser { get; set; } = null!;
    }
}
