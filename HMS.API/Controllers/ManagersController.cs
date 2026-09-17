using HMS.Application.Common;
using HMS.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using static HMS.Application.DTOs.ManagerDtos;

namespace HMS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin,Manager")]
    public class ManagersController : ControllerBase
    {
        private readonly IManagerService _managerService;

        public ManagersController(IManagerService managerService)
        {
            _managerService = managerService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<List<ManagerResponseDto>>>> GetAll()
        {
            try
            {
                var managers = await _managerService.GetAllManagersAsync();
                return Ok(ApiResponse<List<ManagerResponseDto>>.SuccessResponse(managers));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<List<ManagerResponseDto>>.ErrorResponse(ex.Message));
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<ManagerResponseDto>>> GetById(int id)
        {
            try
            {
                var manager = await _managerService.GetManagerByIdAsync(id);
                if (manager == null)
                    return NotFound(ApiResponse<ManagerResponseDto>.ErrorResponse("Manager not found"));

                
                var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
                if (userRole == "Manager")
                {
                    var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                    
                }

                return Ok(ApiResponse<ManagerResponseDto>.SuccessResponse(manager));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<ManagerResponseDto>.ErrorResponse(ex.Message));
            }
        }

        [HttpGet("hotel/{hotelId}")]
        public async Task<ActionResult<ApiResponse<List<ManagerResponseDto>>>> GetByHotel(int hotelId)
        {
            try
            {
              
                var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
                if (userRole == "Manager")
                {
                    var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                  
                }

                var managers = await _managerService.GetManagersByHotelAsync(hotelId);
                return Ok(ApiResponse<List<ManagerResponseDto>>.SuccessResponse(managers));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<List<ManagerResponseDto>>.ErrorResponse(ex.Message));
            }
        }

        // Manager creation is now handled via AuthController endpoints.

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<ManagerResponseDto>>> Update(int id, [FromBody] UpdateManagerDto updateManagerDto)
        {
            try
            {
                var manager = await _managerService.UpdateManagerAsync(id, updateManagerDto);
                return Ok(ApiResponse<ManagerResponseDto>.SuccessResponse(manager, "Manager updated successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<ManagerResponseDto>.ErrorResponse(ex.Message));
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse>> Delete(int id)
        {
            try
            {
                var result = await _managerService.DeleteManagerAsync(id);
                if (!result)
                    return NotFound(ApiResponse.ErrorResponse("Manager not found"));
                return Ok(ApiResponse.SuccessResponse("Manager deleted successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse.ErrorResponse(ex.Message));
            }
        }
    }

}
