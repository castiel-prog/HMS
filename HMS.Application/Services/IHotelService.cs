using HMS.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using static HMS.Application.DTOs.FilterDtos;

namespace HMS.Application.Services
{
    public interface IHotelService
    {
        Task<HotelResponseDto?> GetHotelByIdAsync(int id);
        Task<List<HotelResponseDto>> GetAllHotelsAsync();
        Task<List<HotelResponseDto>> SearchHotelsAsync(HotelFilterDto filterDto);
        Task<HotelResponseDto> CreateHotelAsync(CreateHotelDto createHotelDto);
        Task<HotelResponseDto> UpdateHotelAsync(int id, UpdateHotelDto updateHotelDto);
        Task<bool> DeleteHotelAsync(int id);
    }
}
