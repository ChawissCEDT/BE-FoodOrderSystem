using System;

namespace Backend.Dtos
{
    public class MenuItemResponseDto
    {
        public int Id { get; set; }
        public int RestaurantId { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string Category { get; set; } = null!;
        public string ImageUrl { get; set; } = null!;
        public double Price { get; set; }
        public bool Popular { get; set; }
        public bool IsAvailable { get; set; }
    }

    public class CreateMenuItemRequestDto
    {
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string Category { get; set; } = null!;
        public string ImageUrl { get; set; } = null!;
        public double Price { get; set; }
        public bool Popular { get; set; }
    }

    public class UpdateMenuItemRequestDto
    {
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string Category { get; set; } = null!;
        public string ImageUrl { get; set; } = null!;
        public double Price { get; set; }
        public bool Popular { get; set; }
        public bool IsAvailable { get; set; }
    }
}
