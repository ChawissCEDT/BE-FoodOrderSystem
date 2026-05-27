using System;
using System.ComponentModel.DataAnnotations;

namespace Backend.Dtos
{
    public class OrderResponseDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public string Status { get; set; } = null!;
        public string TypeOrPriority { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public int RelatedUserId { get; set; }
        public UserResponseDto RelatedUser { get; set; } = null!;
    }

    public class CreateOrderRequestDto
    {
        [Required(ErrorMessage = "Title (OrderName) is required.")]
        [StringLength(150, MinimumLength = 3, ErrorMessage = "Title must be between 3 and 150 characters.")]
        public string Title { get; set; } = null!;

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "TypeOrPriority (e.g. Delivery/Pickup) is required.")]
        [RegularExpression("^(Delivery|Pickup)$", ErrorMessage = "TypeOrPriority must be either 'Delivery' or 'Pickup'.")]
        public string TypeOrPriority { get; set; } = null!;

        [Required(ErrorMessage = "RelatedUserId is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "RelatedUserId must be a positive integer.")]
        public int RelatedUserId { get; set; }
    }

    public class UpdateOrderRequestDto
    {
        [Required(ErrorMessage = "Title (OrderName) is required.")]
        [StringLength(150, MinimumLength = 3, ErrorMessage = "Title must be between 3 and 150 characters.")]
        public string Title { get; set; } = null!;

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Status is required.")]
        [RegularExpression("^(Pending|Preparing|Completed|Cancelled)$", ErrorMessage = "Status must be 'Pending', 'Preparing', 'Completed', or 'Cancelled'.")]
        public string Status { get; set; } = null!;

        [Required(ErrorMessage = "TypeOrPriority (e.g. Delivery/Pickup) is required.")]
        [RegularExpression("^(Delivery|Pickup)$", ErrorMessage = "TypeOrPriority must be either 'Delivery' or 'Pickup'.")]
        public string TypeOrPriority { get; set; } = null!;

        [Required(ErrorMessage = "RelatedUserId is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "RelatedUserId must be a positive integer.")]
        public int RelatedUserId { get; set; }
    }
}
