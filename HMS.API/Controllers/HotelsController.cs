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
    public class HotelsController : ControllerBase
    {
        private readonly IHotelService _hotelService;

        public HotelsController(IHotelService hotelService)
        {
            _hotelService = hotelService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<List<HotelResponseDto>>>> GetAll()
        {
            try
            {
                var hotels = await _hotelService.GetAllHotelsAsync();
                return Ok(ApiResponse<List<HotelResponseDto>>.SuccessResponse(hotels));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<List<HotelResponseDto>>.ErrorResponse(ex.Message));
            }
        }

        [HttpGet("search")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<List<HotelResponseDto>>>> Search([FromQuery] HotelFilterDto filterDto)
        {
            try
            {
                var hotels = await _hotelService.SearchHotelsAsync(filterDto);
                return Ok(ApiResponse<List<HotelResponseDto>>.SuccessResponse(hotels));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<List<HotelResponseDto>>.ErrorResponse(ex.Message));
            }
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<HotelResponseDto>>> GetById(int id)
        {
            try
            {
                var hotel = await _hotelService.GetHotelByIdAsync(id);
                if (hotel == null)
                    return NotFound(ApiResponse<HotelResponseDto>.ErrorResponse("Hotel not found"));
                return Ok(ApiResponse<HotelResponseDto>.SuccessResponse(hotel));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<HotelResponseDto>.ErrorResponse(ex.Message));
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<HotelResponseDto>>> Create([FromBody] CreateHotelDto createHotelDto)
        {
            try
            {
                var hotel = await _hotelService.CreateHotelAsync(createHotelDto);
                return CreatedAtAction(nameof(GetById), new { id = hotel.HotelId },
                    ApiResponse<HotelResponseDto>.SuccessResponse(hotel, "Hotel created successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<HotelResponseDto>.ErrorResponse(ex.Message));
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<HotelResponseDto>>> Update(int id, [FromBody] UpdateHotelDto updateHotelDto)
        {
            try
            {
                var hotel = await _hotelService.UpdateHotelAsync(id, updateHotelDto);
                return Ok(ApiResponse<HotelResponseDto>.SuccessResponse(hotel, "Hotel updated successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<HotelResponseDto>.ErrorResponse(ex.Message));
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse>> Delete(int id)
        {
            try
            {
                var result = await _hotelService.DeleteHotelAsync(id);
                if (!result)
                    return NotFound(ApiResponse.ErrorResponse("Hotel not found"));
                return Ok(ApiResponse.SuccessResponse("Hotel deleted successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse.ErrorResponse(ex.Message));
            }
        }
        }
}
