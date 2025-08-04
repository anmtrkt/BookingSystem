using BookingSystem.Core.Domain.Entities.Aggregates;
using BookingSystem.Core.Domain.Entities.Institutions;
using BookingSystem.Core.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingSystem.Infrastructure.Services.Interfaces
{
    public interface IRoomService
    {
        public Task<IEnumerable<Room>> GetRoomsAsync();
        public Task<Room> GetRoomAsync(Guid roomId);
        public Task UpdateAsync(Room room);
        public Task<Room> CreateRoomAsync(CreateRoomDto roomDto);

        public Task<Room> CreateRoomAsync(string number, Guid buildingId, Equipment equipment, uint countOfPlaces);
        public Task<Room> CreateRoomAsync(string number, Building building, Equipment equipment, uint countOfPlaces);
        public Task ChangeEquipmentAsync(Room room, Equipment newEquipment);
        public Task ChangeEquipmentAsync(Guid roomId, Equipment newEquipment);
        public Task DeleteAsync(Guid roomId);
        public Task DeleteAsync(Room room);



    }
}
