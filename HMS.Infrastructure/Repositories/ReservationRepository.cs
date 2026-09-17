using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HMS.Application.Repositories;
using HMS.Domain.Entities;
using HMS.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HMS.Infrastructure.Repositories
{
    public class ReservationRepository : IReservationRepository
    {
        private readonly AppDbContext _context;

        public ReservationRepository(AppDbContext context)
        {
            _context = context;
        }

        // Get operations
        public async Task<Reservation?> GetByIdAsync(int id)
        {
            return await _context.Reservations
                .Include(r => r.Guest)
                .Include(r => r.Hotel)
                .Include(r => r.ReservationRooms)
                .FirstOrDefaultAsync(r => r.ReservationId == id);
        }

        public async Task<IEnumerable<Reservation>> GetAllAsync()
        {
            return await _context.Reservations
                .Include(r => r.Guest)
                .Include(r => r.Hotel)
                .Include(r => r.ReservationRooms)
                .ToListAsync();
        }

        public async Task<IEnumerable<Reservation>> GetByHotelIdAsync(int hotelId)
        {
            return await _context.Reservations
                .Where(r => r.HotelId == hotelId)
                .Include(r => r.Guest)
                .Include(r => r.ReservationRooms)
                .ToListAsync();
        }

        public async Task<IEnumerable<Reservation>> GetByGuestIdAsync(int guestId)
        {
            return await _context.Reservations
                .Where(r => r.GuestId == guestId)
                .Include(r => r.Hotel)
                .Include(r => r.ReservationRooms)
                .ToListAsync();
        }

        // Availability checks
        public async Task<List<Room>> GetAvailableRoomsAsync(int hotelId, DateTime checkInDate, DateTime checkOutDate)
        {
            var bookedRoomIds = await _context.ReservationRooms
                .Where(rr => rr.Reservation.HotelId == hotelId &&
                             rr.Reservation.CheckInDate < checkOutDate &&
                             rr.Reservation.CheckOutDate > checkInDate)
                .Select(rr => rr.RoomId)
                .Distinct()
                .ToListAsync();

            return await _context.Rooms
                .Where(r => r.HotelId == hotelId &&
                           r.IsAvailable &&
                           !bookedRoomIds.Contains(r.RoomId))
                .ToListAsync();
        }

        public async Task<bool> IsRoomAvailableAsync(int roomId, DateTime checkInDate, DateTime checkOutDate)
        {
            var hasConflict = await _context.ReservationRooms
                .Where(rr => rr.RoomId == roomId &&
                             rr.Reservation.CheckInDate < checkOutDate &&
                             rr.Reservation.CheckOutDate > checkInDate)
                .AnyAsync();

            return !hasConflict;
        }

        // Overlapping reservations
        public async Task<IEnumerable<Reservation>> GetOverlappingReservationsAsync(int roomId, DateTime checkInDate, DateTime checkOutDate)
        {
            return await _context.Reservations
                .Where(r => r.ReservationRooms.Any(rr => rr.RoomId == roomId) &&
                           r.CheckInDate < checkOutDate &&
                           r.CheckOutDate > checkInDate)
                .Include(r => r.Guest)
                .Include(r => r.ReservationRooms)
                .ToListAsync();
        }

        // Status queries
        public async Task<IEnumerable<Reservation>> GetActiveReservationsAsync(int hotelId)
        {
            var today = DateTime.UtcNow;
            return await _context.Reservations
                .Where(r => r.HotelId == hotelId &&
                           r.CheckInDate <= today &&
                           r.CheckOutDate >= today)
                .Include(r => r.Guest)
                .Include(r => r.ReservationRooms)
                .ToListAsync();
        }

        public async Task<IEnumerable<Reservation>> GetPendingReservationsAsync(int hotelId)
        {
            return await _context.Reservations
                .Where(r => r.HotelId == hotelId && r.Status == "Pending")
                .Include(r => r.Guest)
                .Include(r => r.ReservationRooms)
                .ToListAsync();
        }

        public async Task<IEnumerable<Reservation>> GetByDateRangeAsync(int hotelId, DateTime startDate, DateTime endDate)
        {
            return await _context.Reservations
                .Where(r => r.HotelId == hotelId &&
                           r.CheckInDate >= startDate &&
                           r.CheckOutDate <= endDate)
                .Include(r => r.Guest)
                .Include(r => r.ReservationRooms)
                .ToListAsync();
        }

        // Add/Update/Delete
        public async Task AddAsync(Reservation reservation)
        {
            await _context.Reservations.AddAsync(reservation);
        }

        public void Update(Reservation reservation)
        {
            _context.Reservations.Update(reservation);
        }

        public void Delete(Reservation reservation)
        {
            _context.Reservations.Remove(reservation);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
