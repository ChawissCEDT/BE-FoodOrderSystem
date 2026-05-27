using System;

namespace Backend.Entities
{
    public class MenuItem
    {
        public int Id { get; set; }
        public int RestaurantId { get; set; }
        public Restaurant Restaurant { get; set; } = null!;

        public required string Name { get; set; }
        public required string Description { get; set; }
        public required string Category { get; set; }
        public double Price { get; set; }
        public bool Popular { get; set; }
        public bool IsAvailable { get; set; } = true;
    }
}
