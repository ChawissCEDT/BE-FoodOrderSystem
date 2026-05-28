using System;
using System.ComponentModel.DataAnnotations;

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
        [Required(ErrorMessage = "Name is required.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 100 characters.")]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "Description is required.")]
        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
        public string Description { get; set; } = null!;

        [Required(ErrorMessage = "Category is required.")]
        [StringLength(50, ErrorMessage = "Category cannot exceed 50 characters.")]
        public string Category { get; set; } = null!;

        [Required(ErrorMessage = "ImageUrl is required.")]
        [Url(ErrorMessage = "Invalid Image URL.")]
        public string ImageUrl { get; set; } = null!;

        [Range(0.01, 10000.0, ErrorMessage = "Price must be between 0.01 and 10000.0.")]
        public double Price { get; set; }

        [Required(ErrorMessage = "Popular status is required.")]
        public bool Popular { get; set; }
    }

    public class UpdateMenuItemRequestDto
    {
        [Required(ErrorMessage = "Name is required.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 100 characters.")]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "Description is required.")]
        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
        public string Description { get; set; } = null!;

        [Required(ErrorMessage = "Category is required.")]
        [StringLength(50, ErrorMessage = "Category cannot exceed 50 characters.")]
        public string Category { get; set; } = null!;

        [Required(ErrorMessage = "ImageUrl is required.")]
        [Url(ErrorMessage = "Invalid Image URL.")]
        public string ImageUrl { get; set; } = null!;

        [Range(0.01, 10000.0, ErrorMessage = "Price must be between 0.01 and 10000.0.")]
        public double Price { get; set; }

        [Required(ErrorMessage = "Popular status is required.")]
        public bool Popular { get; set; }

        [Required(ErrorMessage = "Availability is required.")]
        public bool IsAvailable { get; set; }
    }
}
