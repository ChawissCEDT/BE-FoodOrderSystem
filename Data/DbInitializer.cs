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
                        ImageUrl = "https://images.unsplash.com/photo-1504674900247-0877df9cc836?auto=format&fit=crop&w=900&q=80",
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
                        ImageUrl = "https://images.unsplash.com/photo-1621996346565-e3dbc646d9a9?auto=format&fit=crop&w=900&q=80",
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
                        ImageUrl = "https://images.unsplash.com/photo-1568901346375-23c9450c58cd?auto=format&fit=crop&w=500&q=80",
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
                        ImageUrl = "https://images.unsplash.com/photo-1512621776951-a57141f2eefd?auto=format&fit=crop&w=500&q=80",
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
                    new() { Id = 101, RestaurantId = 1, Name = "Basil Chicken Rice", Description = "Holy basil chicken, jasmine rice, crispy egg, and chili fish sauce.", Category = "Rice", ImageUrl = "https://images.unsplash.com/photo-1652265540600-a319e0e4fd81?q=80&w=1074&auto=format&fit=crop&ixlib=rb-4.1.0&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D", Price = 69, Popular = true },
                    new() { Id = 102, RestaurantId = 1, Name = "Fried Rice", Description = "Thai fried rice with jasmine rice, egg, onion, tomato, scallion, and lime.", Category = "Rice", ImageUrl = "https://images.unsplash.com/photo-1603133872878-684f208fb84b?auto=format&fit=crop&w=500&q=80", Price = 69, Popular = true },
                    new() { Id = 103, RestaurantId = 1, Name = "Thai Milk Tea", Description = "Strong tea, creamy milk, and light brown sugar over ice.", Category = "Drink", ImageUrl = "https://images.unsplash.com/photo-1644031995386-fe9665dc5b57?q=80&w=1974&auto=format&fit=crop&ixlib=rb-4.1.0&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D", Price = 40, Popular = false },
 
                    // Pasta Lab
                    new() { Id = 201, RestaurantId = 2, Name = "Carbonara Bowl", Description = "Cream sauce, smoked bacon, parmesan, black pepper, and spaghetti.", Category = "Pasta", ImageUrl = "https://images.unsplash.com/photo-1627207644206-a2040d60ecad?q=80&w=687&auto=format&fit=crop&ixlib=rb-4.1.0&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D", Price = 139, Popular = true },
                    new() { Id = 202, RestaurantId = 2, Name = "Pomodoro Pasta", Description = "Tomato sauce, garlic, basil, olive oil, and parmesan.", Category = "Pasta", ImageUrl = "https://plus.unsplash.com/premium_photo-1664478291780-0c67f5fb15e6?q=80&w=880&auto=format&fit=crop&ixlib=rb-4.1.0&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D", Price = 119, Popular = false },
                    new() { Id = 203, RestaurantId = 2, Name = "Garlic Bread", Description = "Toasted bread with garlic butter and herbs.", Category = "Side", ImageUrl = "https://plus.unsplash.com/premium_photo-1711752902734-a36167479983?q=80&w=688&auto=format&fit=crop&ixlib=rb-4.1.0&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D", Price = 49, Popular = false },
 
                    // Burger Yard
                    new() { Id = 301, RestaurantId = 3, Name = "Classic Beef Burger", Description = "Beef patty, cheddar, pickles, onion, lettuce, and house sauce.", Category = "Burger", ImageUrl = "https://images.unsplash.com/photo-1568901346375-23c9450c58cd?auto=format&fit=crop&w=500&q=80", Price = 149, Popular = true },
                    new() { Id = 302, RestaurantId = 3, Name = "Chicken Bites", Description = "Crispy chicken pieces with honey mustard dip.", Category = "Side", ImageUrl = "https://images.unsplash.com/photo-1619221881739-40de2afeaa7d?q=80&w=687&auto=format&fit=crop&ixlib=rb-4.1.0&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D", Price = 95, Popular = false },
                    new() { Id = 303, RestaurantId = 3, Name = "Loaded Fries", Description = "Fries with cheese sauce, bacon bits, and scallion.", Category = "Side", ImageUrl = "https://images.unsplash.com/photo-1700835880456-2e5519fa54d6?q=80&w=687&auto=format&fit=crop&ixlib=rb-4.1.0&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D", Price = 65, Popular = true },
 
                    // Green Bowl
                    new() { Id = 401, RestaurantId = 4, Name = "Salmon Protein Bowl", Description = "Grilled salmon, brown rice, edamame, corn, and sesame dressing.", Category = "Bowl", ImageUrl = "https://images.unsplash.com/photo-1512621776951-a57141f2eefd?auto=format&fit=crop&w=500&q=80", Price = 169, Popular = true },
                    new() { Id = 402, RestaurantId = 4, Name = "Avocado Salad", Description = "Mixed greens, avocado, tomato, cucumber, and lemon vinaigrette.", Category = "Salad", ImageUrl = "https://images.unsplash.com/photo-1512621776951-a57141f2eefd?auto=format&fit=crop&w=500&q=80", Price = 129, Popular = false }
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
                        PasswordHash = PasswordHasher.HashPassword("password123"),
                        Role = "Customer"
                    },
                    new()
                    {
                        Id = 2,
                        FullName = "Jane Smith",
                        Email = "jane.smith@example.com",
                        Phone = "0823456789",
                        Address = "456 Sukhumvit Rd, Khlong Toei, Bangkok 10110",
                        PasswordHash = PasswordHasher.HashPassword("password123"),
                        Role = "Customer"
                    },
                    new()
                    {
                        Id = 3,
                        FullName = "Bob Johnson",
                        Email = "bob.johnson@example.com",
                        Phone = "0834567890",
                        Address = "789 Phahonyothin Rd, Chatuchak, Bangkok 10900",
                        PasswordHash = PasswordHasher.HashPassword("password123"),
                        Role = "Customer"
                    },
                    new()
                    {
                        Id = 4,
                        FullName = "System Admin",
                        Email = "admin@example.com",
                        Phone = "0845678901",
                        Address = "System HQ, Bangkok",
                        PasswordHash = PasswordHasher.HashPassword("admin123"),
                        Role = "Admin"
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
