using System;
using System.Collections.Generic;
using System.Text;
using static HMS.Application.DTOs.IAuthDtos;

namespace HMS.Application.Services
{
    public interface IAuthService
    {
        Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto);
        Task<AuthResponseDto> RegisterGuestAsync(RegisterDto registerDto);
        Task<AuthResponseDto> RegisterAdminAsync(RegisterDto registerDto);
        Task<AuthResponseDto> RegisterManagerAsync(RegisterDto registerDto);
        Task<AuthResponseDto> LoginAsync(LoginDto loginDto);
        Task<UserResponseDto?> GetUserByIdAsync(int userId);
        Task<bool> ValidateTokenAsync(string token);
    }
}
