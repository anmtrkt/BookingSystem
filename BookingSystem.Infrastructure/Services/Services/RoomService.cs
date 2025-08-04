using BookingSystem.Core.Domain.Entities.Aggregates;
using BookingSystem.Core.Domain.Entities.Institutions;
using BookingSystem.Core.Domain.Entities.Users;
using BookingSystem.Core.Domain.ValueObjects;
using BookingSystem.Infrastructure.Persistence;
using BookingSystem.Infrastructure.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BookingSystem.Infrastructure.Services.Services
{
    public class RoomService : IRoomService
    {
   /*     public readonly IBuildingService _buildingService;*/
       
        public readonly BookingSystemDbContext _dbContext;

        public RoomService(BookingSystemDbContext dbContext)//,  IBuildingService buildingService)
        {
            _dbContext = dbContext;
      
            /*_buildingService = buildingService;*/
         
        }
        public async Task<IEnumerable<Room>> GetRoomsAsync()
        {
            return await _dbContext.Rooms.ToListAsync();
        }
        public async Task<Room> CreateRoomAsync(CreateRoomDto roomDto)
        {
            return await CreateRoomAsync(roomDto.Number, roomDto.BuildingId, roomDto.Equipment, roomDto.CountOfPlaces);
        }

        public async Task ChangeEquipmentAsync(Room room, Equipment newEquipment)
        {
            room.UpdateEquipment(newEquipment);
            _dbContext.Rooms.Update(room);
            await _dbContext.SaveChangesAsync();
        }
        
        /// <param name="roomId"></param>
        /// <param name="newEquipment"></param>
        /// <returns></returns>
        /// <exception cref="KeyNotFoundException"></exception>
        public async Task ChangeEquipmentAsync(Guid roomId, Equipment newEquipment)
        {
            var room = await _dbContext.Rooms.FindAsync(roomId);
            if (room == null) throw new KeyNotFoundException($"There is no such a {roomId}");
            await ChangeEquipmentAsync(room, newEquipment);
        }
        
        /// <param name="roomId"></param>
        /// <returns></returns>
        /// <exception cref="KeyNotFoundException"></exception>
        public async Task DeleteAsync(Guid roomId)
        {
            var room = await _dbContext.Rooms.FindAsync(roomId);
            if (room == null) throw new KeyNotFoundException($"There is no such a {roomId}");

            _dbContext.Rooms.Remove(room);
            await _dbContext.SaveChangesAsync();
        }
        public async Task DeleteAsync(Room room)
        {
            _dbContext.Rooms.Remove(room);
            await _dbContext.SaveChangesAsync();
        }

        /// <param name="number"></param>
        /// <param name="buildingId"></param>
        /// <param name="equipment"></param>
        /// <param name="countOfPlaces"></param>
        /// <returns></returns>
        /// <exception cref="KeyNotFoundException"></exception>
        public async Task<Room> CreateRoomAsync(string number, Guid buildingId, Equipment equipment, uint countOfPlaces)
        {
            var building = await _dbContext.Buildings.FindAsync(buildingId);
            if (building == null) throw new KeyNotFoundException($"Building with ID: {buildingId} not found.");

            return await CreateRoomAsync(number, building, equipment, countOfPlaces);
        }

        public async Task<Room> CreateRoomAsync(string number, Building building, Equipment equipment, uint countOfPlaces)
        {
            var room = Room.Create(number, building, equipment, countOfPlaces);
            await _dbContext.Rooms.AddAsync(room);
            await _dbContext.SaveChangesAsync();
            building.AddRoom(room); 
            return room;
        }
        public async Task UpdateAsync(Room room)
        {
            _dbContext.Entry(room).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
        }
        /// <param name="roomId"></param>
        /// <returns></returns>
        /// <exception cref="KeyNotFoundException"></exception>
        public async Task<Room> GetRoomAsync(Guid roomId)
        {
            var room = await _dbContext.Rooms.FindAsync(roomId);
            if (room == null) throw new KeyNotFoundException($"Room with ID: {roomId} not found.");

            return room;
        }
        public async Task<bool> IsRoomAvailableAsync(Guid roomId, TimeRange timeRange)
        {

            /*var room = await _roomRepository.GetByIdAsync(roomId);
            if (room == null) return false;
            if (room.IsAvailable == false) return false;
            var meetings = await _meetingRepository.GetMeetingsByRoomAsync(roomId);
            for (int i = 0; i < meetings.Count; i++)
            {
                if (meetings[i].TimeRange.IsOverlapping(timeRange))
                {
                    return false;
                }
            }*/

            return await Task.FromResult(true);
        }
/*        public async Task<bool> IsRoomAvailableAsync(Room room, TimeRange timeRange)
        {
            if (room.IsAvailable == false) return false;
            var meetings = await _meetingService.GetMeetingsByRoomAsync(room.Id);

            for (int i = 0; i < meetings.Count; i++)
            {
                if (meetings[i].TimeRange.IsOverlapping(timeRange))
                {
                    return false;
                }
            }

            return true;
        }
        public async Task<bool> IsRoomAvailableAsync(User user, TimeRange timeRange, Room room)
        {
            if (room.IsAvailable == false) return false;
            var meetings = await _meetingService.GetMeetingsByRoomAsync(room.Id);
            var userPriority = user.Institution.PriorityLevel;
            if (meetings != null)
            {
                for (int i = 0; i < meetings.Count; i++)
                {
                    if (meetings[i].TimeRange.IsOverlapping(timeRange))
                    {
*//*                        if (meetings[i].Institution.PriorityLevel.IsHigherThan(userPriority))
                        {
                            return false;
                        }*//*
                    }
                }
                return true;
            }
            else return true;
        }*/
    }
}
