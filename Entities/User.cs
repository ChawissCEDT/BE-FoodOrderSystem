using System.Collections.Generic;

namespace Backend.Entities
{
    public class User
    {
        public int Id { get; set; }
        public required string FullName { get; set; }
        public required string Email { get; set; }
        public required string Phone { get; set; }
        public required string Address { get; set; }
        public required string PasswordHash { get; set; }
        public string Role { get; set; } = "Customer";

        // Navigation property for related orders
        public ICollection<Order> Orders { get; set; } = new List<Order>();

        // Navigation property for related cart items
        public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
    }
}
