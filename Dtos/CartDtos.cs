using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Backend.Dtos
{
    public class CartItemRequestDto
    {
        [Required(ErrorMessage = "UserId is required.")]
        public int UserId { get; set; }

        [Required(ErrorMessage = "MenuItemId is required.")]
        public int MenuItemId { get; set; }

        [Required(ErrorMessage = "Quantity is required.")]
        public int Quantity { get; set; }
    }

    public class CartMergeRequestDto
    {
        [Required(ErrorMessage = "UserId is required.")]
        public int UserId { get; set; }

        [Required(ErrorMessage = "Items are required.")]
        public List<CartItemRequestDto> Items { get; set; } = new();
    }

    public class CartItemResponseDto
    {
        public MenuItemResponseDto Item { get; set; } = null!;
        public RestaurantResponseDto Restaurant { get; set; } = null!;
        public int Quantity { get; set; }
    }
}
