using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using HMS.Application.DTOs;

namespace HMS.Application.Services
{
    public interface IGuestService
    {
        Task<GuestResponseDto?> GetGuestByIdAsync(int id);
        Task<List<GuestResponseDto>> GetAllGuestsAsync();
        Task<GuestResponseDto> CreateGuestAsync(CreateGuestDto createGuestDto);
        Task<GuestResponseDto> UpdateGuestAsync(int id, UpdateGuestDto updateGuestDto);
        Task<bool> DeleteGuestAsync(int id);
    }
}
