using BookingSystem.Core.Domain.Entities;
using BookingSystem.Core.Domain.Entities.Aggregates;
using BookingSystem.Core.Domain.Entities.Institutions;
using BookingSystem.Core.Domain.Entities.Users;
using BookingSystem.Core.Domain.Models.MeetingsModels;
using BookingSystem.Core.Domain.ValueObjects;
using BookingSystem.Infrastructure.Persistence;
using BookingSystem.Infrastructure.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingSystem.Infrastructure.Services.Services
{
    public class MeetingService : IMeetingService
    {

        private readonly BookingSystemDbContext _dbContext;
        public MeetingService(BookingSystemDbContext dbContext)
        {
            _dbContext = dbContext;

            

        }
        
        /// <param name="userId"></param>
        /// <returns></returns>
        /// <exception cref="KeyNotFoundException"></exception>
        public async Task<List<MeetingDto>> GetMeetingsByUserAsync(Guid userId)
        {
            var meeting = await _dbContext.Meetings.Where(m => m.CreatorId == userId).ToListAsync();
            if (meeting.Count == 0) throw new KeyNotFoundException($"there is no such a Meetings {userId}"); 
            return Meeting.TransformToDto(meeting);
 
        }
        public async Task<bool> CheckIfCanBook(Room room, User user, Schedule schedule, DateTime startTime, DateTime endTime)
        {
            
            for(int i = 0; i < schedule.TimeRanges.Count; i++)
            {
                if (schedule.TimeRanges.ElementAt(i).IsOverlapping(startTime, endTime))
                {
                    var meetingAtThisTimeRange = await _dbContext.Meetings.FirstOrDefaultAsync(m=>m.Id == schedule.TimeRanges.ElementAt(i).MeetingId);
                    var creator = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == meetingAtThisTimeRange.CreatorId);
                    bool iCan = CheckUserPriority(user, creator);
                    if (iCan) { _dbContext.Meetings.Remove(meetingAtThisTimeRange);
                        return CheckUserPriority(user, creator);
                    };
                    return false;
                }
            }
            return true;
        }
        public bool CheckUserPriority(User tryingToCreate, User alreadyCreator)
        {
            var institution1 = _dbContext.Institutions.FirstOrDefault(i => i.Id == tryingToCreate.InstitutionId);
            var institution2 = _dbContext.Institutions.FirstOrDefault(i => i.Id == alreadyCreator.InstitutionId);
            if (institution1.PriorityLevel.IsEqual(institution2.PriorityLevel) == -1) return false;
            if (institution1.PriorityLevel.IsEqual(institution2.PriorityLevel) == 1) return true;
            else
            {
                if(tryingToCreate.PostPriority.IsEqual(alreadyCreator.PostPriority) == -1) return false;
                if (tryingToCreate.PostPriority.IsEqual(alreadyCreator.PostPriority) == 1) return true;
                return false;
            };

        }
        /// <returns></returns>
        /// <exception cref="KeyNotFoundException"></exception>
        public async Task<List<MeetingDto>> GetMeetingsAsync()
        {
            var meetings = await _dbContext.Meetings.ToListAsync();
            if(meetings.Count == 0) throw new KeyNotFoundException("there is no Meetings");
            return Meeting.TransformToDto(meetings);
        }
        
        /// <param name="meetingId"></param>
        /// <returns></returns>
        /// <exception cref="KeyNotFoundException"></exception>
        public async Task DeleteAsync(Guid meetingId)
        {
            var meeting = await _dbContext.Meetings.FindAsync(meetingId);
            if (meeting == null) throw new KeyNotFoundException($"there is no such a Meeting {meetingId}");
            await DeleteAsync(meeting);
        }
        public async Task UpdateAsync(Meeting meeting)
        {
            _dbContext.Entry(meeting).State = EntityState.Modified;
 
            await _dbContext.SaveChangesAsync();
        }

        /// <param name="meetingDto"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        /// <exception cref="KeyNotFoundException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public async Task<MeetingDto?> CreateMeetingAsync(MeetingCreatedDto meetingDto, User user)
        {
            
            var room = await _dbContext.Rooms.FindAsync(meetingDto.RoomId);
            if (room == null) throw new KeyNotFoundException($"Unable to create Meeting: Can not find a Room by given ID{meetingDto.RoomId}");
            var inst = await _dbContext.Institutions.FindAsync(meetingDto.InstitutionId);
            if (inst == null) throw new KeyNotFoundException($"Unable to create Meeting: Can not find a Institution by given ID{meetingDto.InstitutionId}");
            var schedule = await _dbContext.Schedules.Include(s => s.TimeRanges).Where(s => s.Id == room.ScheduleId).FirstOrDefaultAsync();
            if (schedule == null) throw new KeyNotFoundException($"Unable to create Meeting: Can not find a room Schedule by given ID{room.ScheduleId}");
            bool iCan = await CheckIfCanBook(room, user, schedule, meetingDto.StartTime, meetingDto.EndTime);
            if (!iCan) { return null; }


            var meeting = Meeting.Create(user, room, meetingDto.StartTime, meetingDto.EndTime, inst);
            

           
            schedule.AddTime(meeting.TimeRange);
            await _dbContext.Meetings.AddAsync(meeting);
            await _dbContext.SaveChangesAsync();
            _dbContext.Schedules.Update(schedule);
           
    
            await _dbContext.SaveChangesAsync();
            return Meeting.TransformToDto(meeting);
        }

        public async Task<MeetingDto> CreateMeetingAsync(User user, Room room, DateTime start, DateTime end, Branch branch, List<User>? subs = null)
        {
            var meeting = Meeting.Create(user, room, start, end, branch.Institution, subs);
            await _dbContext.Meetings.AddAsync(meeting);
            await _dbContext.SaveChangesAsync();
            return Meeting.TransformToDto(meeting);

        }
        
        /// <param name="userId"></param>
        /// <param name="roomId"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="branchId"></param>
        /// <param name="subsId"></param>
        /// <returns></returns>
        /// <exception cref="KeyNotFoundException"></exception>
        public async Task<MeetingDto> CreateMeetingAsync(Guid userId, Guid roomId, DateTime start, DateTime end, Guid branchId, List<Guid>? subsId = null)
        {
            var user = await _dbContext.Users.FindAsync(userId);
            if (user == null) throw new KeyNotFoundException($"Unable to create Meeting: Can not find a Room by given ID{userId}");

            var room = await _dbContext.Rooms.FindAsync(roomId);
            if (room == null) throw new KeyNotFoundException($"Unable to create Meeting: Can not find a Room by given ID{roomId}");

            var branch = await _dbContext.Branches.FindAsync(branchId);
            if (branch == null) throw new KeyNotFoundException($"Unable to create Meeting: Can not find a Room by given ID{branchId}");

            List<User>? subs = null;
            if (subsId != null)
            {
                subs = await _dbContext.Users
                .Where(u => subsId.Contains(u.Id)).ToListAsync();
            }

            return await CreateMeetingAsync(user, room, start, end, branch, subs);
        }

        public async Task DeleteAsync(Meeting meeting)
        {
            _dbContext.Meetings.Remove(meeting);
            await _dbContext.SaveChangesAsync();
        }

        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="KeyNotFoundException"></exception>
        public async Task<MeetingDto> GetMeetingAsync(Guid id)
        {
            var meeting = await _dbContext.Meetings.FindAsync(id);
            if (meeting == null) throw new KeyNotFoundException($"There is no such a Meeting {id}");
            return Meeting.TransformToDto(meeting);
        }
        public async Task<bool> IsInRequested(Guid userId, Guid meetingId) 
        { 
            var userRequest = await _dbContext.UserSubscribeRequests.Where(r => r.MeetingId == meetingId).ToListAsync();
            foreach (var request in userRequest)
            {
                if (request.UserId == userId) return true;
            }
            return false;
        }
        public async Task CreateRequestAsync(Guid userId, Guid meetingId)
        {
            var userRequest = new UserSubscribeRequest() { MeetingId = meetingId, UserId = userId };
            await _dbContext.UserSubscribeRequests.AddAsync(userRequest);
            await _dbContext.SaveChangesAsync();
        }
       
        /// <param name="userId"></param>
        /// <param name="meetingId"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="KeyNotFoundException"></exception>
        public async Task SubscribeToMeetingAsync(Guid userId, Guid meetingId)
        {
            if (!await IsInRequested(userId, meetingId)) throw new InvalidOperationException("You are not allowed to this meeting");
            var meeting = await _dbContext.Meetings.FindAsync(meetingId);
            if (meeting == null) throw new KeyNotFoundException($"There is no such a Meeting {meetingId}");

            var user = await _dbContext.Users.FindAsync(userId);
            if (user == null) throw new KeyNotFoundException($"There is no such a User {userId}");

            meeting.Subscribers.Add(user);
            await UpdateAsync(meeting);
      
        }
        public async Task<List<MeetingDto>> GetMeetingsByRoomAsync(Guid roomId)
        {
            return Meeting.TransformToDto(await _dbContext.Meetings.Where(m => m.RoomId == roomId).ToListAsync());
        }
        public async Task AddAsync(Meeting meeting)
        {
            await _dbContext.Meetings.AddAsync(meeting);
        }
        public bool Any(Func<Meeting, bool> predicate)
        {
            return _dbContext.Meetings.Any(m => predicate(m));
        }

    }
}
