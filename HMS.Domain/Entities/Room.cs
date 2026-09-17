using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace HMS.Domain.Entities
{
    public class Room
    {
        [Key]
        public int RoomId { get; set; }

        [Required]
        [StringLength(10)]
        public string RoomNumber { get; set; } = null!;

        [Required]
        [StringLength(50)]
        public string Type { get; set; } = null!;

        [Required]
        [Range(1, 10)]
        public int Capacity { get; set; }

        [Required]
        [Range(0, 10000)]
        public decimal PricePerNight { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }

        [Required]
        public bool IsAvailable { get; set; } = true;

        public int Floor { get; set; }

        // Foreign Key
        [ForeignKey(nameof(Hotel))]
        public int HotelId { get; set; }

        // Navigation Properties
        public Hotel Hotel { get; set; } = null!;
        public ICollection<ReservationRoom> ReservationRooms { get; set; } = new List<ReservationRoom>();

    }
}
