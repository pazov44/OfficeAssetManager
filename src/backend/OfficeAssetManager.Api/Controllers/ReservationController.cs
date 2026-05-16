using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OfficeAssetManager.Api.Helpers;
using OfficeAssetManager.Core.DTO;
using OfficeAssetManager.Core.ServiceContracts;
using System.Security.Claims;

namespace OfficeAssetManager.API.Controllers;

public class ReservationController : ApiControllerBase
{
    private readonly IReservationService _reservationService;

    public ReservationController(IReservationService reservationService)
    {
        _reservationService = reservationService;
    }

    [HttpPost]
    public async Task<ActionResult<ReservationResponseDto>> Create(ReservationRequestDto dto)
    {
        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();

            int userId = int.Parse(userIdClaim.Value);

            var result = await _reservationService.CreateReservationAsync(userId, dto);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("my-reservations")]
    public async Task<ActionResult<IEnumerable<ReservationResponseDto>>> GetMyReservations()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null) return Unauthorized();

        int userId = int.Parse(userIdClaim.Value);
        var reservations = await _reservationService.GetUserReservationsAsync(userId);
        return Ok(reservations);
    }

    [Authorize(Roles = "Admin")] // Only Admins can see everyone's bookings
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ReservationResponseDto>>> GetAll()
    {
        var reservations = await _reservationService.GetAllReservationsAsync();
        return Ok(reservations);
    }

    [Authorize(Roles = "Admin")]
    [HttpPatch("{id}/status")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] string status)
    {
        var success = await _reservationService.UpdateStatusAsync(id, status);
        if (!success) return NotFound(new { message = "Reservation not found or invalid status" });
        return Ok(new { message = $"Reservation status updated to {status}" });
    }

    [HttpDelete("{id}/cancel")]
    public async Task<IActionResult> Cancel(int id)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null) return Unauthorized();

        int userId = int.Parse(userIdClaim.Value);
        var success = await _reservationService.CancelReservationAsync(id, userId);

        if (!success) return BadRequest(new { message = "Could not cancel reservation" });
        return NoContent();
    }
}