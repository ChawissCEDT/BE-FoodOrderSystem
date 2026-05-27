using System;
using System.Collections.Generic;

namespace Backend.Entities
{
    public class Restaurant
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Cuisine { get; set; }
        public required string Description { get; set; }
        public double Rating { get; set; }
        public required string DeliveryTime { get; set; }
        public double DeliveryFee { get; set; }
        public required string ImageTone { get; set; }
        public bool IsOpen { get; set; }

        // Navigation property for menu items
        public ICollection<MenuItem> MenuItems { get; set; } = new List<MenuItem>();
    }
}
