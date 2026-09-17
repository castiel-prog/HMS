using HMS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace HMS.Application.Repositories
{
    public interface IReservationRepository
    {
        Task<Reservation?> GetByIdAsync(int id);
        Task<IEnumerable<Reservation>> GetAllAsync();
        Task<IEnumerable<Reservation>> GetByHotelIdAsync(int hotelId);
        Task<IEnumerable<Reservation>> GetByGuestIdAsync(int guestId);

        // Availability checks
        Task<List<Room>> GetAvailableRoomsAsync(int hotelId, DateTime checkInDate, DateTime checkOutDate);
        Task<bool> IsRoomAvailableAsync(int roomId, DateTime checkInDate, DateTime checkOutDate);

        // Overlapping reservations
        Task<IEnumerable<Reservation>> GetOverlappingReservationsAsync(int roomId, DateTime checkInDate, DateTime checkOutDate);

        // Status queries
        Task<IEnumerable<Reservation>> GetActiveReservationsAsync(int hotelId);
        Task<IEnumerable<Reservation>> GetPendingReservationsAsync(int hotelId);
        Task<IEnumerable<Reservation>> GetByDateRangeAsync(int hotelId, DateTime startDate, DateTime endDate);

        // Add/Update/Delete
        Task AddAsync(Reservation reservation);
        void Update(Reservation reservation);
        void Delete(Reservation reservation);
        Task SaveChangesAsync();
    }
}
