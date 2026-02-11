using BookingSystem.Application.DTOs;
using BookingSystem.Application.Exceptions;
using BookingSystem.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BookingSystem.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class BookingController : ControllerBase
{
    private readonly IBookingService _bookingService;

    public BookingController(IBookingService bookingService)
    {
        _bookingService = bookingService;
    }

    [HttpPost]
    public async Task<ActionResult<BookingDto>> Create([FromBody] CreateBookingRequest request)
    {


            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (Guid.TryParse(userIdString, out var userId))
            {
                request.UserId = userId;
            }

            var booking = await _bookingService.CreateBookingAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = booking.Id }, booking);
        
      
    }

    [HttpGet("my")]
    public async Task<ActionResult<IEnumerable<BookingDto>>> GetMyBookings()
    {
        var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(userIdString, out var userId)) return BadRequest();

        var bookings = await _bookingService.GetBookingsByUserIdAsync(userId);
        return Ok(bookings);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<BookingDto>> GetById(Guid id)
    {
        return Ok(await _bookingService.GetBookingByIdAsync(id)); 
    }

    [HttpPost("{id}/cancel")]
    public async Task<IActionResult> Cancel(Guid id)
    {
       
            var booking = await _bookingService.GetBookingByIdAsync(id);
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            // Проверка прав: отменить можно только СВОЮ бронь, если ты не Админ
            if (booking.CreatorId.ToString() != userIdString && userRole != "Admin")
            {
                return Forbid("Нельзя отменять чужие бронирования.");
            }

            await _bookingService.CancelBookingAsync(id);
            return NoContent();
      
    }
}
