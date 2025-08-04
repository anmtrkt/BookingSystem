using BookingSystem.Core.Domain.Entities.Institutions;
using BookingSystem.Core.Domain.Models.BuildingModels;
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
    public class BuildingService : IBuildingService
    {

        public readonly BookingSystemDbContext _dbContext;
        public BuildingService(BookingSystemDbContext dbContext) {
            _dbContext = dbContext;


        }
        public async Task<IEnumerable<Building>> GetBuildingsAsync()
        {
            return await _dbContext.Buildings.ToListAsync();
        }

        /// <param name="createBuildingDto"></param>
        /// <returns></returns>
        /// <exception cref="KeyNotFoundException"></exception>
        public async Task<Building> CreateBuildingAsync(CreateBuildingDto createBuildingDto)
        {
            var branch = await _dbContext.Branches.FindAsync(createBuildingDto.BranchId);
            if (branch == null) throw new KeyNotFoundException($"there is no such Branch {createBuildingDto.BranchId}");
            var building = Building.Create(createBuildingDto.Address, branch);
            await _dbContext.Buildings.AddAsync(building);
            branch.AddBuilding(building);
            await _dbContext.SaveChangesAsync();
            return building;
        }
        public async Task UpdateBuildingAsync(Building building)
        {
            _dbContext.Entry(building).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
        }

        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="KeyNotFoundException"></exception>
        public async Task DeleteBuildingAsync(Guid id) {
            var building = await _dbContext.Buildings.FindAsync(id);
            if (building == null) throw new KeyNotFoundException($"there is no such Building {id}");
            await DeleteBuildingAsync(building);
        }
        public async Task DeleteBuildingAsync(Building building)
        {
            _dbContext.Buildings.Remove(building);
            await _dbContext.SaveChangesAsync();
        }

        /// <param name="buildingId"></param>
        /// <returns></returns>
        /// <exception cref="KeyNotFoundException"></exception>
        public async Task<Building> GetBuildingAsync(Guid buildingId)
        {
            var building = await _dbContext.Buildings.FindAsync(buildingId);
            if (building == null) throw new KeyNotFoundException($"there is no such Building {buildingId}");

            return building;
        }
        public async Task<List<Room>> GetAllRoomAsync(Guid buildingId)
        {
            return await _dbContext.Rooms.Where(r => r.BuildingId == buildingId).ToListAsync();
        }
        public async Task<List<Room>> GetAllRoomAsync(Building building)
        {
            return await GetAllRoomAsync(building.Id);
        }
    }
}
