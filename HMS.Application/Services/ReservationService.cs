using AutoMapper;
using HMS.Application.DTOs;
using HMS.Application.Repositories;
using HMS.Domain.Entities;
using HMS.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HMS.Application.Services
{
    public class ReservationService : IReservationService
    {
        private readonly IReservationRepository _reservationRepository;
        private readonly IRepository<Room> _roomRepository;
        private readonly IRepository<Guest> _guestRepository;
        private readonly IRepository<Hotel> _hotelRepository;
        private readonly IRepository<ReservationRoom> _reservationRoomRepository;
        private readonly IMapper _mapper;

        public ReservationService(IReservationRepository reservationRepository, IRepository<Room> roomRepository,
            IRepository<Guest> guestRepository, IRepository<Hotel> hotelRepository,
            IRepository<ReservationRoom> reservationRoomRepository, IMapper mapper)
        {
            _reservationRepository = reservationRepository;
            _roomRepository = roomRepository;
            _guestRepository = guestRepository;
            _hotelRepository = hotelRepository;
            _reservationRoomRepository = reservationRoomRepository;
            _mapper = mapper;
        }

        public async Task<ReservationResponseDto?> GetReservationByIdAsync(int id)
        {
            var reservation = await _reservationRepository.GetByIdAsync(id);
            return reservation == null ? null : _mapper.Map<ReservationResponseDto>(reservation);
        }

        public async Task<List<ReservationResponseDto>> GetAllReservationsAsync()
        {
            var reservations = await _reservationRepository.GetAllAsync();
            return _mapper.Map<List<ReservationResponseDto>>(reservations.ToList());
        }

        public async Task<List<ReservationResponseDto>> GetReservationsByGuestAsync(int guestId)
        {
            var reservations = await _reservationRepository.GetByGuestIdAsync(guestId);
            return _mapper.Map<List<ReservationResponseDto>>(reservations.ToList());
        }

        public async Task<List<ReservationResponseDto>> GetReservationsByHotelAsync(int hotelId)
        {
            var reservations = await _reservationRepository.GetByHotelIdAsync(hotelId);
            return _mapper.Map<List<ReservationResponseDto>>(reservations.ToList());
        }

        public async Task<List<ReservationResponseDto>> GetActiveReservationsAsync()
        {
            var now = DateTime.UtcNow;
            var reservations = await _reservationRepository.GetAllAsync();
            var active = reservations.Where(r =>
                r.CheckInDate <= now &&
                r.CheckOutDate > now &&
                (r.Status == "Confirmed" || r.Status == "Checked-In"));
            return _mapper.Map<List<ReservationResponseDto>>(active.ToList());
        }

        public async Task<List<ReservationResponseDto>> GetFutureReservationsAsync()
        {
            var now = DateTime.UtcNow;
            var reservations = await _reservationRepository.GetAllAsync();
            var future = reservations.Where(r =>
                r.CheckInDate > now &&
                (r.Status == "Confirmed" || r.Status == "Pending"));
            return _mapper.Map<List<ReservationResponseDto>>(future.ToList());
        }

        public async Task<List<ReservationResponseDto>> GetCompletedReservationsAsync()
        {
            var now = DateTime.UtcNow;
            var reservations = await _reservationRepository.GetAllAsync();
            var completed = reservations.Where(r =>
                r.CheckOutDate <= now || r.Status == "Cancelled");
            return _mapper.Map<List<ReservationResponseDto>>(completed.ToList());
        }

        public async Task<ReservationResponseDto> CreateReservationAsync(CreateReservationDto createReservationDto)
        {
            // Validate room IDs provided
            if (!createReservationDto.RoomIds.Any())
                throw new Exception("At least one room must be selected");

            // Validate check-in is today or later
            if (createReservationDto.CheckInDate.Date < DateTime.UtcNow.Date)
                throw new Exception("Check-in date must be today or later");

            // Validate check-out is after check-in
            if (createReservationDto.CheckOutDate <= createReservationDto.CheckInDate)
                throw new Exception("Check-out date must be after check-in date");

            // Verify guest exists
            var guest = await _guestRepository.GetByIdAsync(createReservationDto.GuestId);
            if (guest == null)
                throw new Exception("Guest not found");

            // Verify hotel exists
            var hotel = await _hotelRepository.GetByIdAsync(createReservationDto.HotelId);
            if (hotel == null)
                throw new Exception("Hotel not found");

            // Verify all rooms and check availability
            decimal totalPrice = 0;
            var numberOfNights = (int)(createReservationDto.CheckOutDate - createReservationDto.CheckInDate).TotalDays;

            foreach (var roomId in createReservationDto.RoomIds)
            {
                var room = await _roomRepository.GetByIdAsync(roomId);
                if (room == null)
                    throw new Exception($"Room with id {roomId} not found");

                if (room.HotelId != createReservationDto.HotelId)
                    throw new Exception($"Room {room.RoomNumber} does not belong to this hotel");

                // Check if room is available
                var isAvailable = await _reservationRepository.IsRoomAvailableAsync(
                    roomId, createReservationDto.CheckInDate, createReservationDto.CheckOutDate);

                if (!isAvailable)
                    throw new Exception($"Room {room.RoomNumber} is not available for selected dates");

                totalPrice += room.PricePerNight * numberOfNights;
            }

            // Create reservation
            var reservation = new Reservation
            {
                GuestId = createReservationDto.GuestId,
                HotelId = createReservationDto.HotelId,
                CheckInDate = createReservationDto.CheckInDate,
                CheckOutDate = createReservationDto.CheckOutDate,
                Status = "Confirmed",
                TotalPrice = totalPrice,
                SpecialRequests = createReservationDto.SpecialRequests,
                ReservedAt = DateTime.UtcNow
            };

            await _reservationRepository.AddAsync(reservation);
            await _reservationRepository.SaveChangesAsync();

            // Add rooms to reservation
            foreach (var roomId in createReservationDto.RoomIds)
            {
                var room = await _roomRepository.GetByIdAsync(roomId);
                var reservationRoom = new ReservationRoom
                {
                    ReservationId = reservation.ReservationId,
                    RoomId = roomId,
                    PricePerNight = room.PricePerNight
                };
                await _reservationRoomRepository.AddAsync(reservationRoom);
            }

            await _reservationRoomRepository.SaveChangesAsync();

            return _mapper.Map<ReservationResponseDto>(reservation);
        }

        public async Task<ReservationResponseDto> UpdateReservationAsync(int id, UpdateReservationDto updateReservationDto)
        {
            var reservation = await _reservationRepository.GetByIdAsync(id);
            if (reservation == null)
                throw new Exception("Reservation not found");

            if (reservation.Status == "Cancelled")
                throw new Exception("Cannot update cancelled reservation");

            var newCheckIn = updateReservationDto.CheckInDate ?? reservation.CheckInDate;
            var newCheckOut = updateReservationDto.CheckOutDate ?? reservation.CheckOutDate;

            // Validate new dates
            if (newCheckIn.Date < DateTime.UtcNow.Date)
                throw new Exception("Check-in date must be today or later");

            if (newCheckOut <= newCheckIn)
                throw new Exception("Check-out date must be after check-in date");

            // If dates changed, recheck room availability
            if (updateReservationDto.CheckInDate.HasValue || updateReservationDto.CheckOutDate.HasValue)
            {
                var reservationRooms = await _reservationRoomRepository.FindAsync(rr => rr.ReservationId == id);

                foreach (var resRoom in reservationRooms)
                {
                    var overlapping = await _reservationRepository.GetOverlappingReservationsAsync(
                        resRoom.RoomId, newCheckIn, newCheckOut);

                    // Filter out the current reservation
                    var conflicts = overlapping.Where(r => r.ReservationId != id).ToList();

                    if (conflicts.Any())
                    {
                        var room = await _roomRepository.GetByIdAsync(resRoom.RoomId);
                        throw new Exception($"Room {room.RoomNumber} is not available for new dates");
                    }
                }
            }

            // Update fields
            if (updateReservationDto.CheckInDate.HasValue)
                reservation.CheckInDate = updateReservationDto.CheckInDate.Value;

            if (updateReservationDto.CheckOutDate.HasValue)
                reservation.CheckOutDate = updateReservationDto.CheckOutDate.Value;

            if (!string.IsNullOrEmpty(updateReservationDto.SpecialRequests))
                reservation.SpecialRequests = updateReservationDto.SpecialRequests;

            _reservationRepository.Update(reservation);
            await _reservationRepository.SaveChangesAsync();

            return _mapper.Map<ReservationResponseDto>(reservation);
        }

        public async Task<bool> CancelReservationAsync(int id)
        {
            var reservation = await _reservationRepository.GetByIdAsync(id);
            if (reservation == null)
                return false;

            if (reservation.Status == "Cancelled")
                throw new Exception("Reservation already cancelled");

            if (reservation.Status == "Checked-In")
                throw new Exception("Cannot cancel checked-in reservation");

            reservation.Status = "Cancelled";
            _reservationRepository.Update(reservation);
            await _reservationRepository.SaveChangesAsync();

            return true;
        }
    }
}
