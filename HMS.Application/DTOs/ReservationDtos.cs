using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace HMS.Application.DTOs
{
    public class ReservationDtos
    {
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public int GuestId { get; set; }
        public int HotelId { get; set; }
        public List<int> RoomIds { get; set; } = new();
        public string? SpecialRequests { get; set; }
    }

    // Alias/Create DTO expected by mappings and services
    public class CreateReservationDto
    {
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public int GuestId { get; set; }
        public int HotelId { get; set; }
        public List<int> RoomIds { get; set; } = new();
        public string? SpecialRequests { get; set; }
    }

    public class UpdateReservationDto
    {
        public DateTime? CheckInDate { get; set; }
        public DateTime? CheckOutDate { get; set; }
        public string? SpecialRequests { get; set; }
    }

    public class ReservationResponseDto
    {
        public int ReservationId { get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public string Status { get; set; } = null!;
        public decimal TotalPrice { get; set; }
        public string? SpecialRequests { get; set; }
        public DateTime ReservedAt { get; set; }
        public int GuestId { get; set; }
        public int HotelId { get; set; }
        public GuestResponseDto? Guest { get; set; }
        public HotelResponseDto? Hotel { get; set; }
        public List<ReservationRoomDto> Rooms { get; set; } = new();
    }
    public class ReservationRoomDto
    {
        public int ReservationRoomId { get; set; }
        public int RoomId { get; set; }
        public string RoomNumber { get; set; } = null!;
        public decimal PricePerNight { get; set; }
        public int NumberOfNights { get; set; }
    }

    public class ReservationFilterDto
    {

        public int? HotelId { get; set; }
        public int? GuestId { get; set; }
        public string? Status { get; set; } // Active Future, "Completed"
    }
}
