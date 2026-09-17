using AutoMapper;
using HMS.Application.DTOs;
using HMS.Domain.Entities;
using HMS.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static HMS.Application.DTOs.FilterDtos;

namespace HMS.Application.Services
{
    public class HotelService : IHotelService
    {
        private readonly IRepository<Hotel> _hotelRepository;
        private readonly IRepository<Room> _roomRepository;
        private readonly IRepository<Reservation> _reservationRepository;
        private readonly IMapper _mapper;

        public HotelService(IRepository<Hotel> hotelRepository, IRepository<Room> roomRepository,
            IRepository<Reservation> reservationRepository, IMapper mapper)
        {
            _hotelRepository = hotelRepository;
            _roomRepository = roomRepository;
            _reservationRepository = reservationRepository;
            _mapper = mapper;
        }

        public async Task<HotelResponseDto?> GetHotelByIdAsync(int id)
        {
            var hotel = await _hotelRepository.GetByIdAsync(id);
            return hotel == null ? null : _mapper.Map<HotelResponseDto>(hotel);
        }

        public async Task<List<HotelResponseDto>> GetAllHotelsAsync()
        {
            var hotels = await _hotelRepository.GetAllAsync();
            return _mapper.Map<List<HotelResponseDto>>(hotels);
        }

        public async Task<List<HotelResponseDto>> SearchHotelsAsync(HotelFilterDto filterDto)
        {
            var hotels = await _hotelRepository.GetAllAsync();

            var filteredHotels = hotels.AsEnumerable();

            if (!string.IsNullOrEmpty(filterDto.Country))
                filteredHotels = filteredHotels.Where(h => h.Address.Contains(filterDto.Country, StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrEmpty(filterDto.City))
                filteredHotels = filteredHotels.Where(h => h.Address.Contains(filterDto.City, StringComparison.OrdinalIgnoreCase));

            if (filterDto.MinRating.HasValue)
                filteredHotels = filteredHotels.Where(h => h.Rating >= filterDto.MinRating);

            return _mapper.Map<List<HotelResponseDto>>(filteredHotels.ToList());
        }

        public async Task<HotelResponseDto> CreateHotelAsync(CreateHotelDto createHotelDto)
        {
            // Validation
            if (string.IsNullOrWhiteSpace(createHotelDto.Name))
                throw new Exception("Hotel name is required");

            if (string.IsNullOrWhiteSpace(createHotelDto.Address))
                throw new Exception("Hotel address is required");

            if (createHotelDto.Rating.HasValue && (createHotelDto.Rating < 1 || createHotelDto.Rating > 5))
                throw new Exception("Rating must be between 1 and 5");

            var hotel = _mapper.Map<Hotel>(createHotelDto);
            await _hotelRepository.AddAsync(hotel);
            await _hotelRepository.SaveChangesAsync();

            return _mapper.Map<HotelResponseDto>(hotel);
        }

        public async Task<HotelResponseDto> UpdateHotelAsync(int id, UpdateHotelDto updateHotelDto)
        {
            var hotel = await _hotelRepository.GetByIdAsync(id);
            if (hotel == null)
                throw new Exception($"Hotel with id {id} not found");

            // Only allow updating specific fields
            if (!string.IsNullOrEmpty(updateHotelDto.Name))
                hotel.Name = updateHotelDto.Name;

            if (!string.IsNullOrEmpty(updateHotelDto.Address))
                hotel.Address = updateHotelDto.Address;

            if (updateHotelDto.Rating.HasValue)
            {
                if (updateHotelDto.Rating < 1 || updateHotelDto.Rating > 5)
                    throw new Exception("Rating must be between 1 and 5");
                hotel.Rating = updateHotelDto.Rating;
            }

            _hotelRepository.Update(hotel);
            await _hotelRepository.SaveChangesAsync();

            return _mapper.Map<HotelResponseDto>(hotel);
        }

        public async Task<bool> DeleteHotelAsync(int id)
        {
            var hotel = await _hotelRepository.GetByIdAsync(id);
            if (hotel == null)
                return false;

            // Check if hotel has rooms
            var rooms = await _roomRepository.FindAsync(r => r.HotelId == id);
            if (rooms.Any())
                throw new Exception("Cannot delete hotel with rooms. Delete all rooms first.");

            // Check if hotel has active reservations
            var activeReservations = await _reservationRepository.FindAsync(r =>
                r.HotelId == id && (r.Status == "Confirmed" || r.Status == "Pending" || r.Status == "Checked-In"));
            if (activeReservations.Any())
                throw new Exception("Cannot delete hotel with active reservations.");

            _hotelRepository.Delete(hotel);
            await _hotelRepository.SaveChangesAsync();
            return true;
        }
    }
}