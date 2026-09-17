using System;
using System.Collections.Generic;
using System.Text;
using static HMS.Application.DTOs.ManagerDtos;

namespace HMS.Application.Services
{
    public interface IManagerService
    {
        Task<ManagerResponseDto?> GetManagerByIdAsync(int id);
        Task<List<ManagerResponseDto>> GetAllManagersAsync();
        Task<List<ManagerResponseDto>> GetManagersByHotelAsync(int hotelId);
        Task<ManagerResponseDto> CreateManagerAsync(CreateManagerDto createManagerDto);
        Task<ManagerResponseDto> UpdateManagerAsync(int id, UpdateManagerDto updateManagerDto);
        Task<bool> DeleteManagerAsync(int id);
    }
}
