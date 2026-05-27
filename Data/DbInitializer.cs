using System;
using System.Linq;
using Backend.Entities;
using Backend.Helpers;
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

            // Force recreation of database schema for clean integration setup
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            // Seed Restaurants
            if (!context.Restaurants.Any())
            {
                var restaurants = new Restaurant[]
                {
                    new()
                    {
                        Id = 1,
                        Name = "Siam Street Kitchen",
                        Cuisine = "Thai comfort food",
                        Description = "Fast campus delivery for rice dishes, curry, noodles, and Thai tea.",
                        Rating = 4.8,
                        DeliveryTime = "20-30 min",
                        DeliveryFee = 35,
                        ImageTone = "thai",
                        IsOpen = true
                    },
                    new()
                    {
                        Id = 2,
                        Name = "Pasta Lab",
                        Cuisine = "Italian bowls",
                        Description = "Creamy pasta, tomato classics, garlic bread, and quick lunch sets.",
                        Rating = 4.6,
                        DeliveryTime = "25-35 min",
                        DeliveryFee = 40,
                        ImageTone = "pasta",
                        IsOpen = true
                    },
                    new()
                    {
                        Id = 3,
                        Name = "Burger Yard",
                        Cuisine = "Burgers and fries",
                        Description = "Stacked burgers, loaded fries, chicken bites, and cold drinks.",
                        Rating = 4.7,
                        DeliveryTime = "18-28 min",
                        DeliveryFee = 30,
                        ImageTone = "burger",
                        IsOpen = true
                    },
                    new()
                    {
                        Id = 4,
                        Name = "Green Bowl",
                        Cuisine = "Healthy meals",
                        Description = "Salads, protein bowls, smoothies, and light dinner options.",
                        Rating = 4.5,
                        DeliveryTime = "22-32 min",
                        DeliveryFee = 25,
                        ImageTone = "green",
                        IsOpen = false
                    }
                };

                context.Restaurants.AddRange(restaurants);
                context.SaveChanges();
            }

            // Seed Menu Items
            if (!context.MenuItems.Any())
            {
                var menuItems = new MenuItem[]
                {
                    // Siam Street Kitchen
                    new() { Id = 101, RestaurantId = 1, Name = "Basil Chicken Rice", Description = "Holy basil chicken, jasmine rice, crispy egg, and chili fish sauce.", Category = "Rice", Price = 89, Popular = true },
                    new() { Id = 102, RestaurantId = 1, Name = "Tom Yum Seafood Noodles", Description = "Spicy-sour broth with shrimp, squid, mushrooms, lime, and roasted chili.", Category = "Noodles", Price = 129, Popular = true },
                    new() { Id = 103, RestaurantId = 1, Name = "Thai Milk Tea", Description = "Strong tea, creamy milk, and light brown sugar over ice.", Category = "Drink", Price = 45, Popular = false },

                    // Pasta Lab
                    new() { Id = 201, RestaurantId = 2, Name = "Carbonara Bowl", Description = "Cream sauce, smoked bacon, parmesan, black pepper, and spaghetti.", Category = "Pasta", Price = 139, Popular = true },
                    new() { Id = 202, RestaurantId = 2, Name = "Pomodoro Pasta", Description = "Tomato sauce, garlic, basil, olive oil, and parmesan.", Category = "Pasta", Price = 119, Popular = false },
                    new() { Id = 203, RestaurantId = 2, Name = "Garlic Bread", Description = "Toasted bread with garlic butter and herbs.", Category = "Side", Price = 59, Popular = false },

                    // Burger Yard
                    new() { Id = 301, RestaurantId = 3, Name = "Classic Beef Burger", Description = "Beef patty, cheddar, pickles, onion, lettuce, and house sauce.", Category = "Burger", Price = 149, Popular = true },
                    new() { Id = 302, RestaurantId = 3, Name = "Chicken Bites", Description = "Crispy chicken pieces with honey mustard dip.", Category = "Side", Price = 95, Popular = false },
                    new() { Id = 303, RestaurantId = 3, Name = "Loaded Fries", Description = "Fries with cheese sauce, bacon bits, and scallion.", Category = "Side", Price = 99, Popular = true },

                    // Green Bowl
                    new() { Id = 401, RestaurantId = 4, Name = "Salmon Protein Bowl", Description = "Grilled salmon, brown rice, edamame, corn, and sesame dressing.", Category = "Bowl", Price = 169, Popular = true },
                    new() { Id = 402, RestaurantId = 4, Name = "Avocado Salad", Description = "Mixed greens, avocado, tomato, cucumber, and lemon vinaigrette.", Category = "Salad", Price = 129, Popular = false }
                };

                context.MenuItems.AddRange(menuItems);
                context.SaveChanges();
            }

            // Seed Users
            if (!context.Users.Any())
            {
                var users = new User[]
                {
                    new()
                    {
                        Id = 1,
                        FullName = "John Doe",
                        Email = "john.doe@example.com",
                        Phone = "0812345678",
                        Address = "123 Siam Sq, Pathum Wan, Bangkok 10330",
                        PasswordHash = PasswordHasher.HashPassword("password123")
                    },
                    new()
                    {
                        Id = 2,
                        FullName = "Jane Smith",
                        Email = "jane.smith@example.com",
                        Phone = "0823456789",
                        Address = "456 Sukhumvit Rd, Khlong Toei, Bangkok 10110",
                        PasswordHash = PasswordHasher.HashPassword("password123")
                    },
                    new()
                    {
                        Id = 3,
                        FullName = "Bob Johnson",
                        Email = "bob.johnson@example.com",
                        Phone = "0834567890",
                        Address = "789 Phahonyothin Rd, Chatuchak, Bangkok 10900",
                        PasswordHash = PasswordHasher.HashPassword("password123")
                    }
                };

                context.Users.AddRange(users);
                context.SaveChanges();
            }

            // Seed Orders
            if (!context.Orders.Any())
            {
                var john = context.Users.First(u => u.Email == "john.doe@example.com");
                var jane = context.Users.First(u => u.Email == "jane.smith@example.com");

                // Get items
                var basilChicken = context.MenuItems.First(m => m.Id == 101);
                var thaiTea = context.MenuItems.First(m => m.Id == 103);
                var carbonara = context.MenuItems.First(m => m.Id == 201);
                var garlicBread = context.MenuItems.First(m => m.Id == 203);

                var r1 = context.Restaurants.First(r => r.Id == 1);
                var r2 = context.Restaurants.First(r => r.Id == 2);

                var o1 = new Order
                {
                    Title = "Lunch from Siam Street Kitchen",
                    Description = "Basil Chicken Rice x2, Thai Milk Tea x1",
                    TypeOrPriority = "Delivery",
                    CustomerName = john.FullName,
                    Phone = john.Phone,
                    Address = john.Address,
                    Note = "No spicy, please.",
                    Status = "Preparing",
                    Subtotal = basilChicken.Price * 2 + thaiTea.Price,
                    DeliveryFee = r1.DeliveryFee,
                    Total = (basilChicken.Price * 2 + thaiTea.Price) + r1.DeliveryFee,
                    RelatedUserId = john.Id,
                    CreatedAt = DateTime.UtcNow.AddMinutes(-20)
                };

                var o2 = new Order
                {
                    Title = "Dinner from Pasta Lab",
                    Description = "Carbonara Bowl x1, Garlic Bread x1",
                    TypeOrPriority = "Pickup",
                    CustomerName = jane.FullName,
                    Phone = jane.Phone,
                    Address = jane.Address,
                    Note = "Ring bell on arrival.",
                    Status = "Completed",
                    Subtotal = carbonara.Price + garlicBread.Price,
                    DeliveryFee = r2.DeliveryFee,
                    Total = (carbonara.Price + garlicBread.Price) + r2.DeliveryFee,
                    RelatedUserId = jane.Id,
                    CreatedAt = DateTime.UtcNow.AddHours(-2)
                };

                context.Orders.AddRange(o1, o2);
                context.SaveChanges();

                var orderItems = new OrderItem[]
                {
                    new() { OrderId = o1.Id, MenuItemId = basilChicken.Id, Name = basilChicken.Name, Price = basilChicken.Price, Quantity = 2 },
                    new() { OrderId = o1.Id, MenuItemId = thaiTea.Id, Name = thaiTea.Name, Price = thaiTea.Price, Quantity = 1 },
                    new() { OrderId = o2.Id, MenuItemId = carbonara.Id, Name = carbonara.Name, Price = carbonara.Price, Quantity = 1 },
                    new() { OrderId = o2.Id, MenuItemId = garlicBread.Id, Name = garlicBread.Name, Price = garlicBread.Price, Quantity = 1 }
                };

                context.OrderItems.AddRange(orderItems);
                context.SaveChanges();
            }
        }
    }
}
