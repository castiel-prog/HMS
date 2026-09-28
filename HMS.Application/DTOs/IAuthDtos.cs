using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace HMS.Application.DTOs
{
    public interface IAuthDtos
    {
        public class RegisterDto
        {
            [Required]
            public string Email { get; set; } = null!;

            [Required]
            public string PhoneNumber { get; set; } = null!;

            [Required]
            public string Password { get; set; } = null!;

            [Required]
            public string ConfirmPassword { get; set; } = null!;

            public string Role { get; set; } = "Guest";

            [Required]
            public string FirstName { get; set; } = null!;

            [Required]
            public string LastName { get; set; } = null!;

            // Optional guest address fields: should appear in Swagger but may be omitted
            public string? Address { get; set; }
            public string? City { get; set; }
            public string? Country { get; set; }
        }

        public class LoginDto
        {
            public string Email { get; set; } = null!;
            public string Password { get; set; } = null!;
        }

        public class AuthResponseDto
        {
            public int UserId { get; set; }
            public string Email { get; set; } = null!;
            public string Role { get; set; } = null!;
            public string Token { get; set; } = null!;
            public DateTime TokenExpiration { get; set; }
        }

        public class UserResponseDto
        {
            public int UserId { get; set; }
            public string Email { get; set; } = null!;
            public string PhoneNumber { get; set; } = null!;
            public string Role { get; set; } = null!;
            public bool IsActive { get; set; }
        }
    }
}
