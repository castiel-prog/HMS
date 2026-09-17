using System;
using System.Collections.Generic;
using System.Text;

namespace HMS.Application.DTOs
{
    public class ManagerDtos
    {
        public class CreateManagerDto
        {
            public string FirstName { get; set; } = null!;
            public string LastName { get; set; } = null!;
            public string PersonalNumber { get; set; } = null!;
            public string Email { get; set; } = null!;
            public string PhoneNumber { get; set; } = null!;
            public int HotelId { get; set; }
            public DateTime HireDate { get; set; }
            public string EmployeeId { get; set; } = null!;
        }

        public class UpdateManagerDto
        {
            public string? FirstName { get; set; }
            public string? LastName { get; set; }
            public string? Email { get; set; }
            public string? PhoneNumber { get; set; }
        }

        public class ManagerResponseDto
        {
            public int ManagerId { get; set; }
            public string FirstName { get; set; } = null!;
            public string LastName { get; set; } = null!;
            public string PersonalNumber { get; set; } = null!;
            public string Email { get; set; } = null!;
            public string PhoneNumber { get; set; } = null!;
            public int HotelId { get; set; }
            public DateTime HireDate { get; set; }
            public string Role { get; set; } = null!;
        }
    }
}
