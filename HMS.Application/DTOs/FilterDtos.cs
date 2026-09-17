using System;
using System.Collections.Generic;
using System.Text;

namespace HMS.Application.DTOs
{
    public class FilterDtos
    {
        public class HotelFilterDto
        {
            public string? Country { get; set; }
            public string? City { get; set; }
            public decimal? MinRating { get; set; }
        }

        public class RoomSearchDto
        {
            public int HotelId { get; set; }
            public DateTime CheckInDate { get; set; }
            public DateTime CheckOutDate { get; set; }
            public decimal? MinPrice { get; set; }
            public decimal? MaxPrice { get; set; }
        }
    }
}
