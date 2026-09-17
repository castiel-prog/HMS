using System;
using System.Collections.Generic;
using System.Text;

namespace HMS.Domain.Entities
{
    public class User
    {
        public int UserId { get; set; }

        public string Email { get; set; } = null!;

        public string PhoneNumber { get; set; } = null!;

        public string PasswordHash { get; set; } = null!;

        public string Role { get; set; } = "Guest"; // Admin, Manager, Guest

        public int? ManagerId { get; set; }
        public int? GuestId { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        public Manager? Manager { get; set; }
        public Guest? Guest { get; set; }
    }
}
