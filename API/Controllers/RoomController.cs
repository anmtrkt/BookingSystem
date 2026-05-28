using BookingSystem.Application.DTOs;
using BookingSystem.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookingSystem.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class RoomController : ControllerBase
{
    private readonly IRoomService _roomService;

    public RoomController(IRoomService roomService)
    {
        _roomService = roomService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<RoomDto>>> GetAll()
    {
        return Ok(await _roomService.GetAllRoomsAsync());
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<RoomDto>> GetById(Guid id)
    { return Ok(await _roomService.GetRoomByIdAsync(id)); 
    }

    [HttpGet("available")]
    public async Task<ActionResult<IEnumerable<RoomDto>>> GetAvailable([FromQuery] DateTime start, [FromQuery] DateTime end)
    {
        var rooms = await _roomService.GetAvailableRoomsAsync(start, end);
        return Ok(rooms);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<RoomDto>> Create([FromBody] CreateRoomRequest request)
    {

        var createdRoom = await _roomService.CreateRoomAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = createdRoom.Id }, createdRoom);
    }

    [HttpPut]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<RoomDto>> Update([FromBody] UpdateRoomRequest request)
    {
        return Ok(await _roomService.UpdateRoomAsync(request)); 
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _roomService.DeleteRoomAsync(id);
        return NoContent();
    }
}
