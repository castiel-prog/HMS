using System;
using System.Collections.Generic;
using System.Text;

namespace HMS.Application.DTOs
{
    public class CreateHotelDto
    {
        public string Name { get; set; } = null!;
        public string Address { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string Email { get; set; } = null!;
        public decimal? Rating { get; set; }
    }

    public class UpdateHotelDto
    {
        public string? Name { get; set; }
        public string? Address { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public decimal? Rating { get; set; }
    }

    public class HotelResponseDto
    {
        public int HotelId { get; set; }
        public string Name { get; set; } = null!;
        public string Address { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string Email { get; set; } = null!;
        public decimal? Rating { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
