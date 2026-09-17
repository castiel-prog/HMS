using HMS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace HMS.Infrastructure.Persistence
{
    public class DatabaseSeeder
    {
        private readonly AppDbContext _context;

        public DatabaseSeeder(AppDbContext context)
        {
            _context = context;
        }

        public async Task SeedAsync()
        {
            try
            {
                // Ensure a single admin user exists so you can log in
                // For local development we always (re)set the admin password to a known value
                // so you can log in reliably. Change this in production.
                var adminEmail = "admin@hms.local";
                var adminPhone = "+1-000-000-0000";
                var adminPassword = "Admin123!";
                var adminPasswordHash = BCrypt.Net.BCrypt.HashPassword(adminPassword);

                var existingAdmin = _context.Users.FirstOrDefault(u => u.Role == "Admin" || u.Email == adminEmail);
                if (existingAdmin == null)
                {
                    var adminUser = new User
                    {
                        Email = adminEmail,
                        PhoneNumber = adminPhone,
                        PasswordHash = adminPasswordHash,
                        Role = "Admin",
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    };

                    _context.Users.Add(adminUser);
                    await _context.SaveChangesAsync();
                    Console.WriteLine($"Seeded admin user: {adminEmail} / {adminPassword}");
                }
                else
                {
                    // Update password hash to known value for dev so login works
                    existingAdmin.PasswordHash = adminPasswordHash;
                    existingAdmin.Email = adminEmail; // normalize email
                    existingAdmin.PhoneNumber = adminPhone;
                    existingAdmin.IsActive = true;
                    _context.Users.Update(existingAdmin);
                    await _context.SaveChangesAsync();
                    Console.WriteLine($"Updated admin password for: {existingAdmin.Email}");
                }

                // If hotels already seeded, skip seeding the rest
                if (_context.Hotels.Any())
                {
                    return; // Data already seeded
                }

                // ==================== HOTELS ====================
                var hotels = new List<Hotel>
            {
                new Hotel
                {
                    Name = "Grand Plaza Hotel",
                    Address = "123 Main Street, New York, NY",
                    PhoneNumber = "+1-212-555-0001",
                    Email = "info@grandplaza.com",
                    Rating = 4.5m,
                    CreatedAt = DateTime.UtcNow
                },
                new Hotel
                {
                    Name = "Sunset Resort",
                    Address = "456 Ocean Avenue, Miami, FL",
                    PhoneNumber = "+1-305-555-0002",
                    Email = "info@sunsetresort.com",
                    Rating = 4.8m,
                    CreatedAt = DateTime.UtcNow
                },
                new Hotel
                {
                    Name = "Mountain View Inn",
                    Address = "789 Alpine Road, Denver, CO",
                    PhoneNumber = "+1-303-555-0003",
                    Email = "info@mountainview.com",
                    Rating = 4.2m,
                    CreatedAt = DateTime.UtcNow
                }
            };

                _context.Hotels.AddRange(hotels);
                await _context.SaveChangesAsync();

                // ==================== MANAGERS ====================
                var managers = new List<Manager>
            {
                new Manager
                {
                    FirstName = "John",
                    LastName = "Anderson",
                    Email = "john.anderson@grandplaza.com",
                    PhoneNumber = "+1-212-555-1001",
                    EmployeeId = "MGR001",
                    HireDate = new DateTime(2020, 1, 15),
                    Role = "Manager",
                    HotelId = hotels[0].HotelId
                },
                new Manager
                {
                    FirstName = "Sarah",
                    LastName = "Johnson",
                    Email = "sarah.johnson@grandplaza.com",
                    PhoneNumber = "+1-212-555-1002",
                    EmployeeId = "MGR002",
                    HireDate = new DateTime(2021, 3, 20),
                    Role = "Manager",
                    HotelId = hotels[0].HotelId
                },
                new Manager
                {
                    FirstName = "Michael",
                    LastName = "Davis",
                    Email = "michael.davis@sunsetresort.com",
                    PhoneNumber = "+1-305-555-2001",
                    EmployeeId = "MGR003",
                    HireDate = new DateTime(2019, 6, 10),
                    Role = "Manager",
                    HotelId = hotels[1].HotelId
                },
                new Manager
                {
                    FirstName = "Emily",
                    LastName = "Wilson",
                    Email = "emily.wilson@mountainview.com",
                    PhoneNumber = "+1-303-555-3001",
                    EmployeeId = "MGR004",
                    HireDate = new DateTime(2022, 2, 5),
                    Role = "Manager",
                    HotelId = hotels[2].HotelId
                }
            };

                _context.Managers.AddRange(managers);
                await _context.SaveChangesAsync();

                // ==================== ROOMS ====================
                var rooms = new List<Room>
            {
                // Grand Plaza Hotel
                new Room
                {
                    RoomNumber = "101",
                    Type = "Single",
                    Capacity = 1,
                    PricePerNight = 89.99m,
                    Description = "Cozy single room with city view",
                    IsAvailable = true,
                    Floor = 1,
                    HotelId = hotels[0].HotelId
                },
                new Room
                {
                    RoomNumber = "102",
                    Type = "Double",
                    Capacity = 2,
                    PricePerNight = 129.99m,
                    Description = "Spacious double room with queen bed",
                    IsAvailable = true,
                    Floor = 1,
                    HotelId = hotels[0].HotelId
                },
                new Room
                {
                    RoomNumber = "201",
                    Type = "Suite",
                    Capacity = 4,
                    PricePerNight = 249.99m,
                    Description = "Luxury suite with living area",
                    IsAvailable = true,
                    Floor = 2,
                    HotelId = hotels[0].HotelId
                },
               
                new Room
                {
                    RoomNumber = "101",
                    Type = "Double",
                    Capacity = 2,
                    PricePerNight = 149.99m,
                    Description = "Double room with ocean view",
                    IsAvailable = true,
                    Floor = 1,
                    HotelId = hotels[1].HotelId
                },
                new Room
                {
                    RoomNumber = "102",
                    Type = "Suite",
                    Capacity = 4,
                    PricePerNight = 299.99m,
                    Description = "Beachfront suite",
                    IsAvailable = true,
                    Floor = 1,
                    HotelId = hotels[1].HotelId
                },
               
                new Room
                {
                    RoomNumber = "101",
                    Type = "Single",
                    Capacity = 1,
                    PricePerNight = 79.99m,
                    Description = "Mountain view single room",
                    IsAvailable = true,
                    Floor = 1,
                    HotelId = hotels[2].HotelId
                },
                new Room
                {
                    RoomNumber = "102",
                    Type = "Double",
                    Capacity = 2,
                    PricePerNight = 119.99m,
                    Description = "Double room with mountain view",
                    IsAvailable = true,
                    Floor = 1,
                    HotelId = hotels[2].HotelId
                }
            };

                _context.Rooms.AddRange(rooms);
                await _context.SaveChangesAsync();

                //  GUESTS //
                var guests = new List<Guest>
            {
                new Guest
                {
                    FirstName = "Robert",
                    LastName = "Martinez",
                    Email = "robert.martinez@email.com",
                    PhoneNumber = "+1-555-0101",
                    Address = "100 Oak Street",
                    City = "New York",
                    Country = "USA",
                    Role = "Guest",
                    RegisteredAt = DateTime.UtcNow
                },
                new Guest
                {
                    FirstName = "Jennifer",
                    LastName = "Taylor",
                    Email = "jennifer.taylor@email.com",
                    PhoneNumber = "+1-555-0102",
                    Address = "200 Pine Avenue",
                    City = "Miami",
                    Country = "USA",
                    Role = "Guest",
                    RegisteredAt = DateTime.UtcNow
                },
                new Guest
                {
                    FirstName = "David",
                    LastName = "Thompson",
                    Email = "david.thompson@email.com",
                    PhoneNumber = "+1-555-0103",
                    Address = "300 Elm Boulevard",
                    City = "Denver",
                    Country = "USA",
                    Role = "Guest",
                    RegisteredAt = DateTime.UtcNow
                },
                new Guest
                {
                    FirstName = "Lisa",
                    LastName = "Garcia",
                    Email = "lisa.garcia@email.com",
                    PhoneNumber = "+1-555-0104",
                    Address = "400 Maple Lane",
                    City = "Los Angeles",
                    Country = "USA",
                    Role = "Guest",
                    RegisteredAt = DateTime.UtcNow
                },
                new Guest
                {
                    FirstName = "James",
                    LastName = "Brown",
                    Email = "james.brown@email.com",
                    PhoneNumber = "+1-555-0105",
                    Address = "500 Cedar Court",
                    City = "Chicago",
                    Country = "USA",
                    Role = "Guest",
                    RegisteredAt = DateTime.UtcNow
                }
            };

                _context.Guests.AddRange(guests);
                await _context.SaveChangesAsync();

                Console.WriteLine("Database seeding completed successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($" Error seeding database: {ex.Message}");
                throw;
            }
        }
}
}