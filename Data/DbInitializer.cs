using System;
using System.Linq;
using Backend.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Backend.Data
{
    public static class DbInitializer
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using var context = new FoodOrderDbContext(
                serviceProvider.GetRequiredService<DbContextOptions<FoodOrderDbContext>>());

            // Creates the database if it does not exist and applies schema
            context.Database.EnsureCreated();

            // Seed Users if none exist
            if (!context.Users.Any())
            {
                var users = new User[]
                {
                    new() { FullName = "John Doe", Email = "john.doe@example.com" },
                    new() { FullName = "Jane Smith", Email = "jane.smith@example.com" },
                    new() { FullName = "Bob Johnson", Email = "bob.johnson@example.com" }
                };

                context.Users.AddRange(users);
                context.SaveChanges();
            }

            // Seed Orders if none exist
            if (!context.Orders.Any())
            {
                var john = context.Users.First(u => u.Email == "john.doe@example.com");
                var jane = context.Users.First(u => u.Email == "jane.smith@example.com");
                var bob = context.Users.First(u => u.Email == "bob.johnson@example.com");

                var orders = new Order[]
                {
                    new()
                    {
                        Title = "Sushi Platter",
                        Description = "Salmon sashimi, tuna rolls, and tempura shrimp",
                        Status = "Pending",
                        TypeOrPriority = "Delivery",
                        RelatedUserId = john.Id,
                        CreatedAt = DateTime.UtcNow.AddMinutes(-45)
                    },
                    new()
                    {
                        Title = "Cheeseburger Meal",
                        Description = "Double beef patty with extra cheese and curly fries",
                        Status = "Preparing",
                        TypeOrPriority = "Delivery",
                        RelatedUserId = jane.Id,
                        CreatedAt = DateTime.UtcNow.AddMinutes(-30)
                    },
                    new()
                    {
                        Title = "Iced Latte & Croissant",
                        Description = "Grande iced vanilla latte with warmed butter croissant",
                        Status = "Completed",
                        TypeOrPriority = "Pickup",
                        RelatedUserId = bob.Id,
                        CreatedAt = DateTime.UtcNow.AddMinutes(-120)
                    },
                    new()
                    {
                        Title = "Phad Thai Noodles",
                        Description = "Spicy shrimp phad thai with extra peanuts",
                        Status = "Cancelled",
                        TypeOrPriority = "Delivery",
                        RelatedUserId = john.Id,
                        CreatedAt = DateTime.UtcNow.AddDays(-1)
                    }
                };

                context.Orders.AddRange(orders);
                context.SaveChanges();
            }
        }
    }
}
