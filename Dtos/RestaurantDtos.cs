using System;

namespace Backend.Dtos
{
    public class RestaurantResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Cuisine { get; set; } = null!;
        public string Description { get; set; } = null!;
        public double Rating { get; set; }
        public string DeliveryTime { get; set; } = null!;
        public double DeliveryFee { get; set; }
        public string ImageTone { get; set; } = null!;
        public bool IsOpen { get; set; }
    }
}
