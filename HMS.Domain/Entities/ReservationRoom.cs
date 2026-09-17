using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace HMS.Domain.Entities
{
    public class ReservationRoom
    {
        [Key]
        public int ReservationRoomId { get; set; }

        [Range(0, 100000)]
        public decimal PricePerNight { get; set; }

        [Range(1, 100)]
        public int NumberOfNights { get; set; }

        // Foreign Keys
        [ForeignKey(nameof(Reservation))]
        public int ReservationId { get; set; }

        [ForeignKey(nameof(Room))]
        public int RoomId { get; set; }

        // Navigation Properties
        public Reservation Reservation { get; set; } = null!;
        public Room Room { get; set; } = null!;
    }
}
