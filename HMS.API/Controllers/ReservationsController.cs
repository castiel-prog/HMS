using HMS.Application.Common;
using HMS.Application.DTOs;
using HMS.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace HMS.Application.Mappings
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ReservationsController : ControllerBase
    {
        private readonly IReservationService _reservationService;

        public ReservationsController(IReservationService reservationService)
        {
            _reservationService = reservationService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<ActionResult<ApiResponse<List<ReservationResponseDto>>>> GetAll()
        {
            try
            {
                var reservations = await _reservationService.GetAllReservationsAsync();
                return Ok(ApiResponse<List<ReservationResponseDto>>.SuccessResponse(reservations));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<List<ReservationResponseDto>>.ErrorResponse(ex.Message));
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<ReservationResponseDto>>> GetById(int id)
        {
            try
            {
                var reservation = await _reservationService.GetReservationByIdAsync(id);
                if (reservation == null)
                    return NotFound(ApiResponse<ReservationResponseDto>.ErrorResponse("Reservation not found"));

                // Guest can only view their own reservations
                var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
                if (userRole == "Guest")
                {
                    var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                    if (reservation.GuestId != userId)
                        return Forbid();
                }

                return Ok(ApiResponse<ReservationResponseDto>.SuccessResponse(reservation));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<ReservationResponseDto>.ErrorResponse(ex.Message));
            }
        }

        [HttpGet("hotel/{hotelId}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<ActionResult<ApiResponse<List<ReservationResponseDto>>>> GetByHotel(int hotelId)
        {
            try
            {
                var reservations = await _reservationService.GetReservationsByHotelAsync(hotelId);
                return Ok(ApiResponse<List<ReservationResponseDto>>.SuccessResponse(reservations));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<List<ReservationResponseDto>>.ErrorResponse(ex.Message));
            }
        }

        [HttpGet("guest/{guestId}")]
        [Authorize(Roles = "Admin,Manager,Guest")]
        public async Task<ActionResult<ApiResponse<List<ReservationResponseDto>>>> GetByGuest(int guestId)
        {
            try
            {
                // Guest can only view their own reservations
                var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
                if (userRole == "Guest")
                {
                    var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                    if (guestId != userId)
                        return Forbid();
                }

                var reservations = await _reservationService.GetReservationsByGuestAsync(guestId);
                return Ok(ApiResponse<List<ReservationResponseDto>>.SuccessResponse(reservations));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<List<ReservationResponseDto>>.ErrorResponse(ex.Message));
            }
        }

        [HttpPost]
        [Authorize(Roles = "Guest")]
        public async Task<ActionResult<ApiResponse<ReservationResponseDto>>> Create([FromBody] CreateReservationDto createReservationDto)
        {
            try
            {
                var reservation = await _reservationService.CreateReservationAsync(createReservationDto);
                return CreatedAtAction(nameof(GetById), new { id = reservation.ReservationId },
                    ApiResponse<ReservationResponseDto>.SuccessResponse(reservation, "Reservation created successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<ReservationResponseDto>.ErrorResponse(ex.Message));
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Manager,Guest")]
        public async Task<ActionResult<ApiResponse<ReservationResponseDto>>> Update(int id, [FromBody] UpdateReservationDto updateReservationDto)
        {
            try
            {
                var reservation = await _reservationService.GetReservationByIdAsync(id);
                if (reservation == null)
                    return NotFound(ApiResponse<ReservationResponseDto>.ErrorResponse("Reservation not found"));

                // Guest can only update their own reservations
                var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
                if (userRole == "Guest")
                {
                    var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                    if (reservation.GuestId != userId)
                        return Forbid();
                }

                var updatedReservation = await _reservationService.UpdateReservationAsync(id, updateReservationDto);
                return Ok(ApiResponse<ReservationResponseDto>.SuccessResponse(updatedReservation, "Reservation updated successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<ReservationResponseDto>.ErrorResponse(ex.Message));
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Manager,Guest")]
        public async Task<ActionResult<ApiResponse>> Cancel(int id)
        {
            try
            {
                var reservation = await _reservationService.GetReservationByIdAsync(id);
                if (reservation == null)
                    return NotFound(ApiResponse.ErrorResponse("Reservation not found"));

                // Guest can only cancel their own reservations
                var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
                if (userRole == "Guest")
                {
                    var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                    if (reservation.GuestId != userId)
                        return Forbid();
                }

                var result = await _reservationService.CancelReservationAsync(id);
                if (!result)
                    return BadRequest(ApiResponse.ErrorResponse("Failed to cancel reservation"));

                return Ok(ApiResponse.SuccessResponse("Reservation cancelled successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse.ErrorResponse(ex.Message));
            }
        }
    }
}