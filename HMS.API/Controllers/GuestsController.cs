
using HMS.Application.Common;
using HMS.Application.DTOs;
using HMS.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HMS.Application.Mappings

{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class GuestsController : ControllerBase
    {
        private readonly IGuestService _guestService;

        public GuestsController(IGuestService guestService)
        {
            _guestService = guestService;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<GuestResponseDto>>>> GetAll()
        {
            try
            {
                var guests = await _guestService.GetAllGuestsAsync();
                return Ok(ApiResponse<List<GuestResponseDto>>.SuccessResponse(guests));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<List<GuestResponseDto>>.ErrorResponse(ex.Message));
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<GuestResponseDto>>> GetById(int id)
        {
            try
            {
                var guest = await _guestService.GetGuestByIdAsync(id);
                if (guest == null)
                    return NotFound(ApiResponse<GuestResponseDto>.ErrorResponse("Guest not found"));
                return Ok(ApiResponse<GuestResponseDto>.SuccessResponse(guest));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<GuestResponseDto>.ErrorResponse(ex.Message));
            }
        }

        // Guest registration is now handled via AuthController endpoints.

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<GuestResponseDto>>> Update(int id, [FromBody] UpdateGuestDto updateGuestDto)
        {
            try
            {
                var guest = await _guestService.UpdateGuestAsync(id, updateGuestDto);
                return Ok(ApiResponse<GuestResponseDto>.SuccessResponse(guest, "Guest updated successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<GuestResponseDto>.ErrorResponse(ex.Message));
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse>> Delete(int id)
        {
            try
            {
                var result = await _guestService.DeleteGuestAsync(id);
                if (!result)
                    return NotFound(ApiResponse.ErrorResponse("Guest not found"));
                return Ok(ApiResponse.SuccessResponse("Guest deleted successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse.ErrorResponse(ex.Message));
            }
        }
    }
}