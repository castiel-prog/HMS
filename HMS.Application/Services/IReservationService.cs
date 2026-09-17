using HMS.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace HMS.Application.Services
{
    public interface IReservationService
    {
        Task<ReservationResponseDto?> GetReservationByIdAsync(int id);
        Task<List<ReservationResponseDto>> GetAllReservationsAsync();
        Task<List<ReservationResponseDto>> GetReservationsByGuestAsync(int guestId);
        Task<List<ReservationResponseDto>> GetReservationsByHotelAsync(int hotelId);
        Task<List<ReservationResponseDto>> GetActiveReservationsAsync();
        Task<List<ReservationResponseDto>> GetFutureReservationsAsync();
        Task<List<ReservationResponseDto>> GetCompletedReservationsAsync();
        Task<ReservationResponseDto> CreateReservationAsync(CreateReservationDto createReservationDto);
        Task<ReservationResponseDto> UpdateReservationAsync(int id, UpdateReservationDto updateReservationDto);
        Task<bool> CancelReservationAsync(int id);
    }
}
