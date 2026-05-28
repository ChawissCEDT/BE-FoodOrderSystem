using System;
using System.ComponentModel.DataAnnotations;

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
        public string ImageUrl { get; set; } = null!;
        public bool IsOpen { get; set; }
    }

    public class CreateRestaurantRequestDto
    {
        [Required(ErrorMessage = "Name is required.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 100 characters.")]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "Cuisine is required.")]
        [StringLength(50, ErrorMessage = "Cuisine cannot exceed 50 characters.")]
        public string Cuisine { get; set; } = null!;

        [Required(ErrorMessage = "Description is required.")]
        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
        public string Description { get; set; } = null!;

        [Range(1.0, 5.0, ErrorMessage = "Rating must be between 1.0 and 5.0.")]
        public double Rating { get; set; } = 5.0;

        [Required(ErrorMessage = "DeliveryTime is required.")]
        [StringLength(50, ErrorMessage = "DeliveryTime description cannot exceed 50 characters.")]
        public string DeliveryTime { get; set; } = null!;

        [Range(0, 1000, ErrorMessage = "DeliveryFee must be between 0 and 1000.")]
        public double DeliveryFee { get; set; }

        [Required(ErrorMessage = "ImageTone is required.")]
        [StringLength(50)]
        public string ImageTone { get; set; } = null!;

        [Required(ErrorMessage = "ImageUrl is required.")]
        [Url(ErrorMessage = "Invalid Image URL.")]
        public string ImageUrl { get; set; } = null!;
    }

    public class UpdateRestaurantRequestDto
    {
        [Required(ErrorMessage = "Name is required.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 100 characters.")]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "Cuisine is required.")]
        [StringLength(50, ErrorMessage = "Cuisine cannot exceed 50 characters.")]
        public string Cuisine { get; set; } = null!;

        [Required(ErrorMessage = "Description is required.")]
        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
        public string Description { get; set; } = null!;

        [Range(1.0, 5.0, ErrorMessage = "Rating must be between 1.0 and 5.0.")]
        public double Rating { get; set; }

        [Required(ErrorMessage = "DeliveryTime is required.")]
        [StringLength(50, ErrorMessage = "DeliveryTime description cannot exceed 50 characters.")]
        public string DeliveryTime { get; set; } = null!;

        [Range(0, 1000, ErrorMessage = "DeliveryFee must be between 0 and 1000.")]
        public double DeliveryFee { get; set; }

        [Required(ErrorMessage = "ImageTone is required.")]
        [StringLength(50)]
        public string ImageTone { get; set; } = null!;

        [Required(ErrorMessage = "ImageUrl is required.")]
        [Url(ErrorMessage = "Invalid Image URL.")]
        public string ImageUrl { get; set; } = null!;

        [Required(ErrorMessage = "IsOpen is required.")]
        public bool IsOpen { get; set; }
    }
}
