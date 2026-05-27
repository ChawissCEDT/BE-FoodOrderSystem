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

        // Navigation property for related orders
        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}
