using HMS.Application.Common;
using HMS.Application.DTOs;
using HMS.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using static HMS.Application.DTOs.FilterDtos;

namespace HMS.Application.Mappings
{
    [ApiController]
    [Route("api/[controller]")]
    public class RoomsController : ControllerBase
    {
        private readonly IRoomService _roomService;

        public RoomsController(IRoomService roomService)
        {
            _roomService = roomService;
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<RoomResponseDto>>> GetById(int id)
        {
            try
            {
                var room = await _roomService.GetRoomByIdAsync(id);
                if (room == null)
                    return NotFound(ApiResponse<RoomResponseDto>.ErrorResponse("Room not found"));
                return Ok(ApiResponse<RoomResponseDto>.SuccessResponse(room));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<RoomResponseDto>.ErrorResponse(ex.Message));
            }
        }

        [HttpGet("hotel/{hotelId}")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<List<RoomResponseDto>>>> GetRoomsByHotel(int hotelId)
        {
            try
            {
                var rooms = await _roomService.GetRoomsByHotelAsync(hotelId);
                return Ok(ApiResponse<List<RoomResponseDto>>.SuccessResponse(rooms));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<List<RoomResponseDto>>.ErrorResponse(ex.Message));
            }
        }

        [HttpPost("search")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<List<RoomResponseDto>>>> Search([FromBody] RoomSearchDto searchDto)
        {
            try
            {
                var rooms = await _roomService.SearchRoomsAsync(searchDto);
                return Ok(ApiResponse<List<RoomResponseDto>>.SuccessResponse(rooms));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<List<RoomResponseDto>>.ErrorResponse(ex.Message));
            }
        }

        [HttpGet("available/{hotelId}")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<List<RoomResponseDto>>>> GetAvailableRooms(int hotelId, [FromQuery] DateTime checkInDate, [FromQuery] DateTime checkOutDate)
        {
            try
            {
                var rooms = await _roomService.GetAvailableRoomsAsync(hotelId, checkInDate, checkOutDate);
                return Ok(ApiResponse<List<RoomResponseDto>>.SuccessResponse(rooms));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<List<RoomResponseDto>>.ErrorResponse(ex.Message));
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<ActionResult<ApiResponse<RoomResponseDto>>> Create([FromBody] CreateRoomDto createRoomDto)
        {
            try
            {
                var room = await _roomService.CreateRoomAsync(createRoomDto);
                return CreatedAtAction(nameof(GetById), new { id = room.RoomId },
                    ApiResponse<RoomResponseDto>.SuccessResponse(room, "Room created successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<RoomResponseDto>.ErrorResponse(ex.Message));
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<ActionResult<ApiResponse<RoomResponseDto>>> Update(int id, [FromBody] UpdateRoomDto updateRoomDto)
        {
            try
            {
                var room = await _roomService.UpdateRoomAsync(id, updateRoomDto);
                return Ok(ApiResponse<RoomResponseDto>.SuccessResponse(room, "Room updated successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<RoomResponseDto>.ErrorResponse(ex.Message));
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse>> Delete(int id)
        {
            try
            {
                var result = await _roomService.DeleteRoomAsync(id);
                if (!result)
                    return NotFound(ApiResponse.ErrorResponse("Room not found"));
                return Ok(ApiResponse.SuccessResponse("Room deleted successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse.ErrorResponse(ex.Message));
            }
        }
        }
}
