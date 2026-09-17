using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace HMS.Domain.Entities
{
    public class Guest
    {
        [Key]
        public int GuestId { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string FirstName { get; set; } = null!;

        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string LastName { get; set; } = null!;

        [Required]
        [StringLength(100)]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [Required]
        [StringLength(20)]
        [Phone]
        public string PhoneNumber { get; set; } = null!;

        [StringLength(200)]
        public string? Address { get; set; }

        [StringLength(50)]
        public string? City { get; set; }

        [StringLength(50)]
        public string? Country { get; set; }


        public DateTime RegisteredAt { get; set; } = DateTime.UtcNow;

        [Required]
        [StringLength(50)]
        public string Role { get; set; } = "Guest";
        // Navigation Property
        public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();

    }
}
