using AutoMapper;
using HMS.Domain.Entities;
using HMS.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Text;
using static HMS.Application.DTOs.ManagerDtos;

namespace HMS.Application.Services
{
    public class ManagerService : IManagerService
    {
        private readonly IRepository<Manager> _managerRepository;
        private readonly IRepository<Hotel> _hotelRepository;
        private readonly IMapper _mapper;

        public ManagerService(IRepository<Manager> managerRepository, IRepository<Hotel> hotelRepository, IMapper mapper)
        {
            _managerRepository = managerRepository;
            _hotelRepository = hotelRepository;
            _mapper = mapper;
        }

        public async Task<ManagerResponseDto?> GetManagerByIdAsync(int id)
        {
            var manager = await _managerRepository.GetByIdAsync(id);
            return manager == null ? null : _mapper.Map<ManagerResponseDto>(manager);
        }

        public async Task<List<ManagerResponseDto>> GetAllManagersAsync()
        {
            var managers = await _managerRepository.GetAllAsync();
            return _mapper.Map<List<ManagerResponseDto>>(managers);
        }

        public async Task<List<ManagerResponseDto>> GetManagersByHotelAsync(int hotelId)
        {
            var managers = await _managerRepository.FindAsync(m => m.HotelId == hotelId);
            return _mapper.Map<List<ManagerResponseDto>>(managers);
        }

        public async Task<ManagerResponseDto> CreateManagerAsync(CreateManagerDto createManagerDto)
        {
            // Validation
            if (string.IsNullOrWhiteSpace(createManagerDto.FirstName))
                throw new Exception("First name is required");

            if (string.IsNullOrWhiteSpace(createManagerDto.LastName))
                throw new Exception("Last name is required");

            if (string.IsNullOrWhiteSpace(createManagerDto.PersonalNumber))
                throw new Exception("Personal number is required");

            if (string.IsNullOrWhiteSpace(createManagerDto.Email))
                throw new Exception("Email is required");

            if (string.IsNullOrWhiteSpace(createManagerDto.PhoneNumber))
                throw new Exception("Phone number is required");

            // Check if hotel exists
            var hotel = await _hotelRepository.GetByIdAsync(createManagerDto.HotelId);
            if (hotel == null)
                throw new Exception($"Hotel with id {createManagerDto.HotelId} not found");

            // Check for duplicate personal number
            var existingPersonalNumber = await _managerRepository.FirstOrDefaultAsync(m =>
                m.PersonalNumber == createManagerDto.PersonalNumber);
            if (existingPersonalNumber != null)
                throw new Exception("Personal number already exists");

            // Check for duplicate email
            var existingEmail = await _managerRepository.FirstOrDefaultAsync(m =>
                m.Email == createManagerDto.Email);
            if (existingEmail != null)
                throw new Exception("Email already exists");

            var manager = _mapper.Map<Manager>(createManagerDto);
            manager.Role = "Manager";

            await _managerRepository.AddAsync(manager);
            await _managerRepository.SaveChangesAsync();

            return _mapper.Map<ManagerResponseDto>(manager);
        }

        public async Task<ManagerResponseDto> UpdateManagerAsync(int id, UpdateManagerDto updateManagerDto)
        {
            var manager = await _managerRepository.GetByIdAsync(id);
            if (manager == null)
                throw new Exception($"Manager with id {id} not found");

            // Check for duplicate email if being updated
            if (!string.IsNullOrEmpty(updateManagerDto.Email) && updateManagerDto.Email != manager.Email)
            {
                var existingEmail = await _managerRepository.FirstOrDefaultAsync(m =>
                    m.Email == updateManagerDto.Email && m.ManagerId != id);
                if (existingEmail != null)
                    throw new Exception("Email already exists");
            }

            if (!string.IsNullOrEmpty(updateManagerDto.FirstName))
                manager.FirstName = updateManagerDto.FirstName;

            if (!string.IsNullOrEmpty(updateManagerDto.LastName))
                manager.LastName = updateManagerDto.LastName;

            if (!string.IsNullOrEmpty(updateManagerDto.Email))
                manager.Email = updateManagerDto.Email;

            if (!string.IsNullOrEmpty(updateManagerDto.PhoneNumber))
                manager.PhoneNumber = updateManagerDto.PhoneNumber;

            _managerRepository.Update(manager);
            await _managerRepository.SaveChangesAsync();

            return _mapper.Map<ManagerResponseDto>(manager);
        }

        public async Task<bool> DeleteManagerAsync(int id)
        {
            var manager = await _managerRepository.GetByIdAsync(id);
            if (manager == null)
                return false;

            // Check if hotel has other managers
            var hotelManagers = await _managerRepository.FindAsync(m => m.HotelId == manager.HotelId);
            if (hotelManagers.Count() <= 1)
                throw new Exception("Cannot delete the only manager. Hotel must have at least one manager.");

            _managerRepository.Delete(manager);
            await _managerRepository.SaveChangesAsync();
            return true;
        }
    }
}