using HMS.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Text;
using static HMS.Application.DTOs.FilterDtos;

namespace HMS.Application.Services
{
    public interface IRoomService
    {
        Task<RoomResponseDto?> GetRoomByIdAsync(int id);
        Task<List<RoomResponseDto>> GetRoomsByHotelAsync(int hotelId);
        Task<List<RoomResponseDto>> GetAvailableRoomsAsync(int hotelId, DateTime checkInDate, DateTime checkOutDate);
        Task<List<RoomResponseDto>> SearchRoomsAsync(RoomSearchDto searchDto);
        Task<RoomResponseDto> CreateRoomAsync(CreateRoomDto createRoomDto);
        Task<RoomResponseDto> UpdateRoomAsync(int id, UpdateRoomDto updateRoomDto);
        Task<bool> DeleteRoomAsync(int id);

    }
}
