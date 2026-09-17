using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace HMS.Domain.Entities
{
    public class Reservation
    {
        [Key]
        public int ReservationId { get; set; }

        [Required]
        public DateTime CheckInDate { get; set; }

        [Required]
        public DateTime CheckOutDate { get; set; }

        [Required]
        [StringLength(50)]
        public string Status { get; set; } = "Pending";

        [Range(0, 100000)]
        public decimal TotalPrice { get; set; }

        [StringLength(500)]
        public string? SpecialRequests { get; set; }

        public DateTime ReservedAt { get; set; } = DateTime.UtcNow;

        // Foreign Keys
        [ForeignKey(nameof(Guest))]
        public int GuestId { get; set; }

        [ForeignKey(nameof(Hotel))]
        public int HotelId { get; set; }

        // Navigation Properties
        public Guest Guest { get; set; } = null!;
        public Hotel Hotel { get; set; } = null!;
        public ICollection<ReservationRoom> ReservationRooms { get; set; } = new List<ReservationRoom>();

    }
}
