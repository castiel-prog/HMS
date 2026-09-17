using AutoMapper;
using BCrypt.Net;
using HMS.Domain.Entities;
using HMS.Infrastructure.Repositories;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using static HMS.Application.DTOs.IAuthDtos;

namespace HMS.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<Guest> _guestRepository;
        private readonly IMapper _mapper;
        private readonly string _jwtSecretKey;
        private readonly string _jwtIssuer;
        private readonly string _jwtAudience;
        private readonly int _jwtExpirationMinutes;

        public AuthService(IRepository<User> userRepository, IRepository<Guest> guestRepository, IMapper mapper,
            string jwtSecretKey, string jwtIssuer, string jwtAudience, int jwtExpirationMinutes)
        {
            _userRepository = userRepository;
            _guestRepository = guestRepository;
            _mapper = mapper;
            _jwtSecretKey = jwtSecretKey;
            _jwtIssuer = jwtIssuer;
            _jwtAudience = jwtAudience;
            _jwtExpirationMinutes = jwtExpirationMinutes;
        }

        public Task<AuthResponseDto> RegisterGuestAsync(RegisterDto registerDto)
        {
            registerDto.Role = "Guest";
            return RegisterAsync(registerDto);
        }

        public async Task<AuthResponseDto> RegisterAdminAsync(RegisterDto registerDto)
        {
            // Only create User with Admin role, no Manager/Guest entity
            registerDto.Role = "Admin";

            // Validate input
            if (string.IsNullOrWhiteSpace(registerDto.Email))
                throw new Exception("Email is required");

            if (registerDto.Password != registerDto.ConfirmPassword)
                throw new Exception("Passwords do not match");

            if (registerDto.Password.Length < 6)
                throw new Exception("Password must be at least 6 characters");

            var existingUser = await _userRepository.FirstOrDefaultAsync(u => u.Email == registerDto.Email);
            if (existingUser != null)
                throw new Exception("Email already registered");

            var existingPhone = await _userRepository.FirstOrDefaultAsync(u => u.PhoneNumber == registerDto.PhoneNumber);
            if (existingPhone != null)
                throw new Exception("Phone number already registered");

            string passwordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password);

            var user = new User
            {
                Email = registerDto.Email,
                PhoneNumber = registerDto.PhoneNumber,
                PasswordHash = passwordHash,
                Role = "Admin",
                IsActive = true
            };

            await _userRepository.AddAsync(user);
            await _userRepository.SaveChangesAsync();

            string token = GenerateJwtToken(user.UserId, user.Email, user.Role);

            return new AuthResponseDto
            {
                UserId = user.UserId,
                Email = user.Email,
                Role = user.Role,
                Token = token,
                TokenExpiration = DateTime.UtcNow.AddMinutes(_jwtExpirationMinutes)
            };
        }

        public async Task<AuthResponseDto> RegisterManagerAsync(RegisterDto registerDto)
        {
            // Manager needs both User and Manager entity
            registerDto.Role = "Manager";

            if (string.IsNullOrWhiteSpace(registerDto.FirstName))
                throw new Exception("First name is required");

            if (string.IsNullOrWhiteSpace(registerDto.LastName))
                throw new Exception("Last name is required");

            // Reuse some validation from RegisterAsync path
            if (registerDto.Password != registerDto.ConfirmPassword)
                throw new Exception("Passwords do not match");

            if (registerDto.Password.Length < 6)
                throw new Exception("Password must be at least 6 characters");

            var existingUser = await _userRepository.FirstOrDefaultAsync(u => u.Email == registerDto.Email);
            if (existingUser != null)
                throw new Exception("Email already registered");

            var existingPhone = await _userRepository.FirstOrDefaultAsync(u => u.PhoneNumber == registerDto.PhoneNumber);
            if (existingPhone != null)
                throw new Exception("Phone number already registered");

            string passwordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password);

            var manager = new Manager
            {
                FirstName = registerDto.FirstName!,
                LastName = registerDto.LastName!,
                Email = registerDto.Email,
                PhoneNumber = registerDto.PhoneNumber,
                PersonalNumber = "",
                EmployeeId = Guid.NewGuid().ToString(),
                HireDate = DateTime.UtcNow,
                Role = "Manager",
                HotelId = 0 
            };

          

            // Create User
            var user = new User
            {
                Email = registerDto.Email,
                PhoneNumber = registerDto.PhoneNumber,
                PasswordHash = passwordHash,
                Role = "Manager",
                IsActive = true
            };

            await _userRepository.AddAsync(user);
            await _userRepository.SaveChangesAsync();


            string token = GenerateJwtToken(user.UserId, user.Email, user.Role);

            return new AuthResponseDto
            {
                UserId = user.UserId,
                Email = user.Email,
                Role = user.Role,
                Token = token,
                TokenExpiration = DateTime.UtcNow.AddMinutes(_jwtExpirationMinutes)
            };
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto)
        {
            // Validate input
            if (string.IsNullOrWhiteSpace(registerDto.Email))
                throw new Exception("Email is required");

            if (registerDto.Password != registerDto.ConfirmPassword)
                throw new Exception("Passwords do not match");

            if (registerDto.Password.Length < 6)
                throw new Exception("Password must be at least 6 characters");

            // Check if email already exists
            var existingUser = await _userRepository.FirstOrDefaultAsync(u => u.Email == registerDto.Email);
            if (existingUser != null)
                throw new Exception("Email already registered");

            // Check if phone already exists
            var existingPhone = await _userRepository.FirstOrDefaultAsync(u => u.PhoneNumber == registerDto.PhoneNumber);
            if (existingPhone != null)
                throw new Exception("Phone number already registered");

            // Hash password
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password);

            // Create user
            var user = new User
            {
                Email = registerDto.Email,
                PhoneNumber = registerDto.PhoneNumber,
                PasswordHash = passwordHash,
                Role = registerDto.Role,
                IsActive = true
            };

            // If registering as Guest, create Guest entity
            if (registerDto.Role == "Guest" && !string.IsNullOrEmpty(registerDto.FirstName))
            {
                var guest = new Guest
                {
                    FirstName = registerDto.FirstName,
                    LastName = registerDto.LastName ?? "",
                    Email = registerDto.Email,
                    PhoneNumber = registerDto.PhoneNumber,
                    Role = "Guest"
                };

                await _guestRepository.AddAsync(guest);
                await _guestRepository.SaveChangesAsync();

                user.GuestId = guest.GuestId;
            }

            await _userRepository.AddAsync(user);
            await _userRepository.SaveChangesAsync();

            // Generate JWT
            string token = GenerateJwtToken(user.UserId, user.Email, user.Role);

            return new AuthResponseDto
            {
                UserId = user.UserId,
                Email = user.Email,
                Role = user.Role,
                Token = token,
                TokenExpiration = DateTime.UtcNow.AddMinutes(_jwtExpirationMinutes)
            };
        }

        public async Task<AuthResponseDto> LoginAsync(LoginDto loginDto)
        {
            // Find user by email
            var user = await _userRepository.FirstOrDefaultAsync(u => u.Email == loginDto.Email);
            if (user == null)
                throw new Exception("Invalid email or password");

            // Verify password
            bool isValidPassword = BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash);
            if (!isValidPassword)
                throw new Exception("Invalid email or password");

            if (!user.IsActive)
                throw new Exception("User account is inactive");

            // Generate JWT
            string token = GenerateJwtToken(user.UserId, user.Email, user.Role);

            return new AuthResponseDto
            {
                UserId = user.UserId,
                Email = user.Email,
                Role = user.Role,
                Token = token,
                TokenExpiration = DateTime.UtcNow.AddMinutes(_jwtExpirationMinutes)
            };
        }

        public async Task<UserResponseDto?> GetUserByIdAsync(int userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            return user == null ? null : _mapper.Map<UserResponseDto>(user);
        }

        public async Task<bool> ValidateTokenAsync(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_jwtSecretKey);

                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = _jwtIssuer,
                    ValidateAudience = true,
                    ValidAudience = _jwtAudience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                return true;
            }
            catch
            {
                return false;
            }
        }

        private string GenerateJwtToken(int userId, string email, string role)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSecretKey);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Email, email),
                new Claim(ClaimTypes.Role, role)
            }),
                Expires = DateTime.UtcNow.AddMinutes(_jwtExpirationMinutes),
                Issuer = _jwtIssuer,
                Audience = _jwtAudience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    
    }
}
