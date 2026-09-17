using HMS.Application.Common;
using HMS.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using static HMS.Application.DTOs.IAuthDtos;

namespace HMS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }



        [HttpPost("register-guest")]
        public async Task<ActionResult<ApiResponse<AuthResponseDto>>> RegisterGuest([FromBody] RegisterDto registerDto)
        {
            try
            {
                var result = await _authService.RegisterGuestAsync(registerDto);
                return Ok(ApiResponse<AuthResponseDto>.SuccessResponse(result, "Guest registration successful"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<AuthResponseDto>.ErrorResponse(ex.Message));
            }
        }

        [HttpPost("register-admin")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<AuthResponseDto>>> RegisterAdmin([FromBody] RegisterDto registerDto)
        {
            try
            {
                var result = await _authService.RegisterAdminAsync(registerDto);
                return Ok(ApiResponse<AuthResponseDto>.SuccessResponse(result, "Admin registration successful"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<AuthResponseDto>.ErrorResponse(ex.Message));
            }
        }

        [HttpPost("register-manager")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<AuthResponseDto>>> RegisterManager([FromBody] RegisterDto registerDto)
        {
            try
            {
                var result = await _authService.RegisterManagerAsync(registerDto);
                return Ok(ApiResponse<AuthResponseDto>.SuccessResponse(result, "Manager registration successful"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<AuthResponseDto>.ErrorResponse(ex.Message));
            }
        }

        [HttpPost("login")]
        public async Task<ActionResult<ApiResponse<AuthResponseDto>>> Login([FromBody] LoginDto loginDto)
        {
            try
            {
                var result = await _authService.LoginAsync(loginDto);
                return Ok(ApiResponse<AuthResponseDto>.SuccessResponse(result, "Login successful"));
            }
            catch (Exception ex)
            {
                return Unauthorized(ApiResponse<AuthResponseDto>.ErrorResponse(ex.Message));
            }
        }
    }
}