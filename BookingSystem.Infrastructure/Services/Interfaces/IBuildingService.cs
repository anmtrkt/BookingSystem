using BookingSystem.Core.Domain.Entities.Institutions;
using BookingSystem.Core.Domain.Models.BuildingModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingSystem.Infrastructure.Services.Interfaces
{
    public interface IBuildingService
    {
        public Task<IEnumerable<Building>> GetBuildingsAsync();
        public Task<Building> CreateBuildingAsync(CreateBuildingDto buildingDto);
        public Task<Building> GetBuildingAsync(Guid id);
        public Task UpdateBuildingAsync(Building building);
        public Task DeleteBuildingAsync(Guid id);
        public Task DeleteBuildingAsync(Building building);
        public Task<List<Room>> GetAllRoomAsync(Guid buildingId);
        public Task<List<Room>> GetAllRoomAsync(Building building);

    }
}
