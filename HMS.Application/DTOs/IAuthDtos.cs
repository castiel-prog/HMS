using System;
using System.Collections.Generic;
using System.Text;

namespace HMS.Application.DTOs
{
    public interface IAuthDtos
    {
        public class RegisterDto
        {
            public string Email { get; set; } = null!;
            public string PhoneNumber { get; set; } = null!;
            public string Password { get; set; } = null!;
            public string ConfirmPassword { get; set; } = null!;
            public string Role { get; set; } = "Guest";
            public string? FirstName { get; set; }
            public string? LastName { get; set; }
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
