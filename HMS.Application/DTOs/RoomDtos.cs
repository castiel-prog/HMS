using System;
using System.Collections.Generic;
using System.Text;

namespace HMS.Application.DTOs
{
    public class CreateRoomDto
    {
        public string RoomNumber { get; set; } = null!;
        public string Type { get; set; } = null!;
        public int Capacity { get; set; }
        public decimal PricePerNight { get; set; }
        public string? Description { get; set; }
        public int Floor { get; set; }
        public int HotelId { get; set; }
    }

    public class UpdateRoomDto
    {
        public string? RoomNumber { get; set; }
        public string? Type { get; set; }
        public int? Capacity { get; set; }
        public decimal? PricePerNight { get; set; }
        public string? Description { get; set; }
        public bool? IsAvailable { get; set; }
        public int? Floor { get; set; }
    }

    public class RoomResponseDto
    {
        public int RoomId { get; set; }
        public string RoomNumber { get; set; } = null!;
        public string Type { get; set; } = null!;
        public int Capacity { get; set; }
        public decimal PricePerNight { get; set; }
        public string? Description { get; set; }
        public bool IsAvailable { get; set; }
        public int Floor { get; set; }
        public int HotelId { get; set; }
    }
}
