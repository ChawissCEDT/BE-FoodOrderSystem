using System;

namespace Backend.Entities
{
    public class OrderItem
    {
        public int Id { get; set; }
        
        public int OrderId { get; set; }
        public Order Order { get; set; } = null!;

        public int? MenuItemId { get; set; }
        public MenuItem? MenuItem { get; set; }

        public required string Name { get; set; } // Snapshot at order time
        public double Price { get; set; }        // Snapshot at order time
        public int Quantity { get; set; }
    }
}
