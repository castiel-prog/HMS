using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace HMS.Domain.Entities
{
    public class Hotel
    {

        [Key]
        public int HotelId { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string Name { get; set; } = null!;

        [Required]
        [StringLength(500)]
        public string Address { get; set; } = null!;

        [Required]
        [StringLength(20)]
        [Phone]
        public string PhoneNumber { get; set; } = null!;

        [Required]
        [StringLength(100)]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [Range(0, 5)]
        public decimal? Rating { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        public ICollection<Manager> Managers { get; set; } = new List<Manager>();
        public ICollection<Room> Rooms { get; set; } = new List<Room>();
        public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
    }

}

