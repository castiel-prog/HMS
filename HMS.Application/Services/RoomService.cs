using AutoMapper;
using HMS.Application.DTOs;
using HMS.Application.Repositories;
using HMS.Domain.Entities;
using HMS.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Text;
using static HMS.Application.DTOs.FilterDtos;

namespace HMS.Application.Services
{
    public class RoomService : IRoomService
    {
        private readonly IRepository<Room> _roomRepository;
        private readonly IRepository<Reservation> _reservationRepository;
        private readonly IReservationRepository _reservationSpecialRepository;
        private readonly IMapper _mapper;

        public RoomService(IRepository<Room> roomRepository, IRepository<Reservation> reservationRepository,
            IReservationRepository reservationSpecialRepository, IMapper mapper)
        {
            _roomRepository = roomRepository;
            _reservationRepository = reservationRepository;
            _reservationSpecialRepository = reservationSpecialRepository;
            _mapper = mapper;
        }

        public async Task<RoomResponseDto?> GetRoomByIdAsync(int id)
        {
            var room = await _roomRepository.GetByIdAsync(id);
            return room == null ? null : _mapper.Map<RoomResponseDto>(room);
        }

        public async Task<List<RoomResponseDto>> GetRoomsByHotelAsync(int hotelId)
        {
            var rooms = await _roomRepository.FindAsync(r => r.HotelId == hotelId);
            return _mapper.Map<List<RoomResponseDto>>(rooms);
        }

        public async Task<List<RoomResponseDto>> GetAvailableRoomsAsync(int hotelId, DateTime checkInDate, DateTime checkOutDate)
        {
            var availableRooms = await _reservationSpecialRepository.GetAvailableRoomsAsync(hotelId, checkInDate, checkOutDate);
            return _mapper.Map<List<RoomResponseDto>>(availableRooms);
        }

        public async Task<List<RoomResponseDto>> SearchRoomsAsync(RoomSearchDto searchDto)
        {
            // Get available rooms for the date range
            var availableRooms = await _reservationSpecialRepository.GetAvailableRoomsAsync(
                searchDto.HotelId, searchDto.CheckInDate, searchDto.CheckOutDate);

            var filteredRooms = availableRooms.AsEnumerable();

            // Apply price filters
            if (searchDto.MinPrice.HasValue)
                filteredRooms = filteredRooms.Where(r => r.PricePerNight >= searchDto.MinPrice);

            if (searchDto.MaxPrice.HasValue)
                filteredRooms = filteredRooms.Where(r => r.PricePerNight <= searchDto.MaxPrice);

            return _mapper.Map<List<RoomResponseDto>>(filteredRooms.ToList());
        }

        public async Task<RoomResponseDto> CreateRoomAsync(CreateRoomDto createRoomDto)
        {
            // Validation
            if (string.IsNullOrWhiteSpace(createRoomDto.RoomNumber))
                throw new Exception("Room number is required");

            if (createRoomDto.PricePerNight <= 0)
                throw new Exception("Room price must be greater than 0");

            if (createRoomDto.Capacity < 1)
                throw new Exception("Room capacity must be at least 1");

            // Check for duplicate room number in same hotel
            var existingRoom = await _roomRepository.FirstOrDefaultAsync(r =>
                r.HotelId == createRoomDto.HotelId && r.RoomNumber == createRoomDto.RoomNumber);
            if (existingRoom != null)
                throw new Exception("Room number already exists in this hotel");

            var room = _mapper.Map<Room>(createRoomDto);
            await _roomRepository.AddAsync(room);
            await _roomRepository.SaveChangesAsync();

            return _mapper.Map<RoomResponseDto>(room);
        }

        public async Task<RoomResponseDto> UpdateRoomAsync(int id, UpdateRoomDto updateRoomDto)
        {
            var room = await _roomRepository.GetByIdAsync(id);
            if (room == null)
                throw new Exception($"Room with id {id} not found");

            // Validate price if being updated
            if (updateRoomDto.PricePerNight.HasValue && updateRoomDto.PricePerNight <= 0)
                throw new Exception("Room price must be greater than 0");

            _mapper.Map(updateRoomDto, room);
            _roomRepository.Update(room);
            await _roomRepository.SaveChangesAsync();

            return _mapper.Map<RoomResponseDto>(room);
        }

        public async Task<bool> DeleteRoomAsync(int id)
        {
            var room = await _roomRepository.GetByIdAsync(id);
            if (room == null)
                return false;

            // Check for active or future reservations
            var futureReservations = await _reservationRepository.FindAsync(r =>
                r.ReservationRooms.Any(rr => rr.RoomId == id) &&
                (r.Status == "Confirmed" || r.Status == "Pending" || r.Status == "Checked-In" ||
                 r.CheckOutDate > DateTime.UtcNow));

            if (futureReservations.Any())
                throw new Exception("Cannot delete room with active or future reservations");

            _roomRepository.Delete(room);
            await _roomRepository.SaveChangesAsync();
            return true;
        }
    }
}