using BookingSystem.Core.Domain.Entities;
using BookingSystem.Core.Domain.Entities.Aggregates;
using BookingSystem.Core.Domain.Entities.Institutions;
using BookingSystem.Core.Domain.Entities.Users;
using BookingSystem.Core.Domain.Models.MeetingsModels;
using BookingSystem.Core.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingSystem.Infrastructure.Services.Interfaces
{
    public interface IMeetingService
    {
        public Task<bool> CheckIfCanBook(Room room, User user, Schedule schedule, DateTime startTime, DateTime endTime);
        public Task<List<MeetingDto>> GetMeetingsByUserAsync(Guid userId);

        public Task CreateRequestAsync(Guid userId, Guid meetingId);
        public Task SubscribeToMeetingAsync(Guid userId, Guid meetingId);

        public Task<bool> IsInRequested(Guid userId, Guid meetingId);

        public Task<List<MeetingDto>> GetMeetingsAsync();

        public Task DeleteAsync(Guid meetingId);

        public Task UpdateAsync(Meeting meeting);

        public Task<MeetingDto> CreateMeetingAsync(MeetingCreatedDto meetingDto, User user);

        public Task<MeetingDto> CreateMeetingAsync(User user, Room room, DateTime start, DateTime end, Branch branch, List<User>? subs = null);

        public Task<MeetingDto> CreateMeetingAsync(Guid userId, Guid roomId, DateTime start, DateTime end, Guid branchId, List<Guid>? subsId = null);


        public Task DeleteAsync(Meeting meeting);

        public Task<MeetingDto> GetMeetingAsync(Guid id);

        public Task<List<MeetingDto>> GetMeetingsByRoomAsync(Guid roomId);

        public Task AddAsync(Meeting meeting);

        public bool Any(Func<Meeting, bool> predicate);

    }
}
