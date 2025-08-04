using BookingSystem.API.Services.Models;
using BookingSystem.Core.Domain.Entities.Aggregates;
using BookingSystem.Core.Domain.Entities.Users;
using BookingSystem.Core.Domain.Models.MeetingsModels;
using BookingSystem.Infrastructure.Persistence;
using BookingSystem.Infrastructure.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace BookingSystem.API.Controllers.BookingControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MeetingsController : ControllerBase
    {
        private readonly IEmailService _emailService;
        private readonly IUserService _userService;
        private readonly IMeetingService _meetingService;
        private readonly UserManager<User> _userManager;

        public MeetingsController(IEmailService emailService, IUserService userService, IMeetingService meetingService, UserManager<User> userManager)
        {
            _emailService = emailService;
            _userService = userService;
            _meetingService = meetingService;
            _userManager = userManager;
        }

        // GET: api/Meetings
        [HttpGet]
        [Authorize]
        [ProducesResponseType(typeof(MeetingDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<IEnumerable<MeetingDto>>> GetMeetings()
        {
            var currentUser = _userManager.GetUserAsync(User).Result;
            if (currentUser == null) { return NotFound(); }
            var meetings = await _meetingService.GetMeetingsByUserAsync(currentUser.Id);
            if (meetings == null) return NoContent();
            return Ok(meetings);
        }

        // GET: api/Meetings/5
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<MeetingDto>> GetMeeting(Guid id)
        {

            var meetings = await GetMeetings();

            if (meetings.Value == null)
            {
                return NotFound();
            }


            return Ok(meetings.Value.FirstOrDefault(m=>m.Id == id));
        }
        // GET: 
        [HttpPost("SendRequest/{userid}+{meetingId}")]
        [Authorize]
        public async Task<ActionResult> SendSubscribeRequestMeeting(Guid userid, Guid meetingId)
        {
            await _meetingService.CreateRequestAsync(userid, meetingId);
            var meeting = await _meetingService.GetMeetingAsync(meetingId);
            var user = await _userService.GetUserAsync(userid);
            await _emailService.SubscribeToBookingMailAsync(user.Email, user.FullName, meeting);
            return Ok("Запрос отправлен");
        }
        [HttpPost("Subscribe/{userid}+{meetingId}")]
        [Authorize]
        public async Task<ActionResult<MeetingDto>> SubscribeMeeting(Guid userid, Guid meetingId)
        {
            try
            {
                await _meetingService.SubscribeToMeetingAsync(userid, meetingId);
            }
            catch (Exception ex)
            {
                BadRequest(ex.Message);
            }
            var meeting = await _meetingService.GetMeetingAsync(meetingId);
            var user = await _userService.GetUserAsync(userid);
            
            return Ok(meeting);
        }

        // PUT: api/Meetings/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> PutMeeting(Guid id, Meeting meeting)
        {
            if (id != meeting.Id)
            {
                return BadRequest();
            }
            

            try
            {
                await _meetingService.UpdateAsync(meeting);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MeetingExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Meetings
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("CreateMeeting")]
        [Authorize]
        [ProducesResponseType(typeof(MeetingDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<MeetingDto>> PostMeeting(MeetingCreatedDto meetingDto)
        {
            try
            {
                var currentUser = _userManager.GetUserAsync(User).Result;
                if (meetingDto.SubscriberIds.Count != 0 && !User.IsInRole("manager")) { return Unauthorized("You dont have permission to do this"); }
                if (currentUser == null) return NotFound();
                var meeting = await _meetingService.CreateMeetingAsync(meetingDto, currentUser);

                return Ok(CreatedAtAction("GetMeeting", new { id = meeting.Id }, meeting));
            }
            catch (Exception)
            {
                return BadRequest("Something went wrong");
            }
  
        }

            

        // DELETE: api/Meetings/5
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteMeeting(Guid id)
        {
            
            await _meetingService.DeleteAsync(id);

            return NoContent();
        }





        private bool MeetingExists(Guid id)
        {
            return _meetingService.Any(e => e.Id == id);
        }



    }
}
