using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookingSystem.Core.Domain.Entities.Institutions;
using BookingSystem.Infrastructure.Persistence;
using BookingSystem.Infrastructure.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using BookingSystem.Core.Domain.Entities.Users;

namespace BookingSystem.API.Controllers.AdminController
{
    [Route("api/[controller]")]
    //[Authorize(Roles =Roles.Admin)]
    [ApiController]
    public class RoomsController : ControllerBase
    {
        private readonly IRoomService _roomService;
        private readonly BookingSystemDbContext _context;

        public RoomsController(BookingSystemDbContext context, IRoomService roomService)
        {
            _roomService = roomService;
            _context = context;
        }

        // GET: api/Rooms
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Room>>> GetRooms()
        {
            return Ok(await _roomService.GetRoomsAsync());
        }

        // GET: api/Rooms/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Room>> GetRoom(Guid id)
        {
            var room = await _roomService.GetRoomAsync(id);

            if (room == null)
            {
                return NotFound();
            }

            return room;
        }

        // PUT: api/Rooms/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRoom(Guid id, Room room)
        {
            if (id != room.Id)
            {
                return BadRequest();
            }
            await _roomService.UpdateAsync(room);
           

            return NoContent();
        }

        // POST: api/Rooms
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Room>> PostRoom([FromBody] CreateRoomDto roomDto)
        {
            var room = await _roomService.CreateRoomAsync(roomDto);

            return CreatedAtAction("GetRoom", new { id = room.Id }, room);
        }

        // DELETE: api/Rooms/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRoom(Guid id)
        {
            var room = await _roomService.GetRoomAsync(id);
            if (room == null)
            {
                return NotFound();
            }

            await _roomService.DeleteAsync(room);

            return NoContent();
        }

    }
}
