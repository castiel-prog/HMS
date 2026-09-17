
using AutoMapper;
using HMS.Application.DTOs;
using HMS.Domain.Entities;
using HMS.Infrastructure.Repositories;
namespace HMS.Application.Services
{
    public class GuestService : IGuestService
    {
        private readonly IRepository<Guest> _guestRepository;
        private readonly IRepository<Reservation> _reservationRepository;
        private readonly IMapper _mapper;

        public GuestService(IRepository<Guest> guestRepository, IRepository<Reservation> reservationRepository, IMapper mapper)
        {
            _guestRepository = guestRepository;
            _reservationRepository = reservationRepository;
            _mapper = mapper;
        }

        public async Task<GuestResponseDto?> GetGuestByIdAsync(int id)
        {
            var guest = await _guestRepository.GetByIdAsync(id);
            return guest == null ? null : _mapper.Map<GuestResponseDto>(guest);
        }

        public async Task<List<GuestResponseDto>> GetAllGuestsAsync()
        {
            var guests = await _guestRepository.GetAllAsync();
            return _mapper.Map<List<GuestResponseDto>>(guests.ToList());
        }

        public async Task<GuestResponseDto> CreateGuestAsync(CreateGuestDto createGuestDto)
        {
            // Validation
            if (string.IsNullOrWhiteSpace(createGuestDto.FirstName))
                throw new Exception("First name is required");

            if (string.IsNullOrWhiteSpace(createGuestDto.LastName))
                throw new Exception("Last name is required");

            if (string.IsNullOrWhiteSpace(createGuestDto.Email))
                throw new Exception("Email is required");

            if (string.IsNullOrWhiteSpace(createGuestDto.PhoneNumber))
                throw new Exception("Phone number is required");

            // Check for duplicate email
            var existingEmail = await _guestRepository.FirstOrDefaultAsync(g => g.Email == createGuestDto.Email);
            if (existingEmail != null)
                throw new Exception("Email already exists");

            // Check for duplicate phone
            var existingPhone = await _guestRepository.FirstOrDefaultAsync(g => g.PhoneNumber == createGuestDto.PhoneNumber);
            if (existingPhone != null)
                throw new Exception("Phone number already exists");

            var guest = _mapper.Map<Guest>(createGuestDto);
            guest.Role = "Guest";
            guest.RegisteredAt = DateTime.UtcNow;

            await _guestRepository.AddAsync(guest);
            await _guestRepository.SaveChangesAsync();

            return _mapper.Map<GuestResponseDto>(guest);
        }

        public async Task<GuestResponseDto> UpdateGuestAsync(int id, UpdateGuestDto updateGuestDto)
        {
            var guest = await _guestRepository.GetByIdAsync(id);
            if (guest == null)
                throw new Exception($"Guest with id {id} not found");

            // Check for duplicate email if being updated
            if (!string.IsNullOrEmpty(updateGuestDto.Email) && updateGuestDto.Email != guest.Email)
            {
                var existingEmail = await _guestRepository.FirstOrDefaultAsync(g =>
                    g.Email == updateGuestDto.Email && g.GuestId != id);
                if (existingEmail != null)
                    throw new Exception("Email already exists");
            }

            // Check for duplicate phone if being updated
            if (!string.IsNullOrEmpty(updateGuestDto.PhoneNumber) && updateGuestDto.PhoneNumber != guest.PhoneNumber)
            {
                var existingPhone = await _guestRepository.FirstOrDefaultAsync(g =>
                    g.PhoneNumber == updateGuestDto.PhoneNumber && g.GuestId != id);
                if (existingPhone != null)
                    throw new Exception("Phone number already exists");
            }

            // Update fields
            if (!string.IsNullOrEmpty(updateGuestDto.FirstName))
                guest.FirstName = updateGuestDto.FirstName;

            if (!string.IsNullOrEmpty(updateGuestDto.LastName))
                guest.LastName = updateGuestDto.LastName;

            if (!string.IsNullOrEmpty(updateGuestDto.Email))
                guest.Email = updateGuestDto.Email;

            if (!string.IsNullOrEmpty(updateGuestDto.PhoneNumber))
                guest.PhoneNumber = updateGuestDto.PhoneNumber;

            if (!string.IsNullOrEmpty(updateGuestDto.Address))
                guest.Address = updateGuestDto.Address;

            if (!string.IsNullOrEmpty(updateGuestDto.City))
                guest.City = updateGuestDto.City;

            if (!string.IsNullOrEmpty(updateGuestDto.Country))
                guest.Country = updateGuestDto.Country;

            _guestRepository.Update(guest);
            await _guestRepository.SaveChangesAsync();

            return _mapper.Map<GuestResponseDto>(guest);
        }

        public async Task<bool> DeleteGuestAsync(int id)
        {
            var guest = await _guestRepository.GetByIdAsync(id);
            if (guest == null)
                return false;

            // Check for active or future reservations
            var activeReservations = await _reservationRepository.FindAsync(r =>
                r.GuestId == id && (r.Status == "Confirmed" || r.Status == "Pending" || r.Status == "Checked-In" ||
                                    r.CheckOutDate > DateTime.UtcNow));

            if (activeReservations.Any())
                throw new Exception("Cannot delete guest with active or future reservations");

            _guestRepository.Delete(guest);
            await _guestRepository.SaveChangesAsync();

            return true;
        }
    } }