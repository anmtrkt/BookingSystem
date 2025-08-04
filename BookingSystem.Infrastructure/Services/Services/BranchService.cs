using BookingSystem.Core.Domain.Entities.Institutions;
using BookingSystem.Core.Domain.Models.BranchModel;
using BookingSystem.Infrastructure.Persistence;
using BookingSystem.Infrastructure.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OpenTelemetry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingSystem.Infrastructure.Services.Services
{
    public class BranchService: IBranchService
    {
/*        public readonly IBuildingService _buildingService;
        public readonly IMeetingService _meetingService;*/
        public readonly BookingSystemDbContext _dbContext;

        public BranchService(BookingSystemDbContext dbContext/*, IMeetingService meetingService, IBuildingService buildingService*/)
        {
            _dbContext = dbContext;
/*            _buildingService = buildingService;
            _meetingService = meetingService;*/
        }
        public async Task<IEnumerable<Branch>> GetBranchesAsync()
        {
            return await _dbContext.Branches.ToListAsync();
        }

        /// <param name="createBranchDto"></param>
        /// <returns></returns>
        /// <exception cref="KeyNotFoundException"></exception>
        public async Task<Branch> CreateBranchAsync(CreateBranchDto createBranchDto)
        {
            var inst = _dbContext.Institutions.Find(createBranchDto.InstitutionId);
            if (inst == null) throw new KeyNotFoundException("There is no such a branch");
            var branch = Branch.Create(inst, createBranchDto.Name, createBranchDto.Address);
            _dbContext.Branches.Add(branch);
        
            inst.Branches.Add(branch);
            

            await _dbContext.SaveChangesAsync();
            return branch;
        }


        /// <param name="branchId"></param>
        /// <returns></returns>
        /// <exception cref="KeyNotFoundException"></exception>
        public async Task<Branch> GetBranchAsync(Guid branchId)
        {
            var branch = await _dbContext.Branches
                .FirstOrDefaultAsync(b => b.Id == branchId);

            if (branch == null)
                throw new KeyNotFoundException("Branch not found.");

            return branch;
        }
        public async Task<List<Building>> GetBuildingsAsync(Guid branchId)
        {
            var buildings = await _dbContext.Buildings.Where(b => b.BranchId == branchId).ToListAsync();
            return buildings;
        }
        public async Task<List<Building>> GetBuildingsAsync(Branch branch)
        {
            return await GetBuildingsAsync(branch.Id);
        }
        public async Task<List<Room>> GetRoomsAsync(Branch branch)
        {
            return await GetRoomsAsync(branch.Id);
        }

        public async Task<List<Room>> GetRoomsAsync(Guid branchId)
        {
            List<Room> rooms = new(16);
            var buildings = await GetBuildingsAsync(branchId);
            foreach (Building building in buildings)
            {
                rooms.AddRange( await _dbContext.Rooms.Where(r => r.BuildingId == building.Id).ToListAsync());   
            }
            return rooms;
        }

        public async Task UpdateAsync(Branch branch)
        {

            _dbContext.Entry(branch).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
        }

        // Удаление
        public async Task DeleteBranchAsync(Guid branchId)
        {
            var branch = await GetBranchAsync(branchId);
            await DeleteBranchAsync(branch);
        }

        public async Task DeleteBranchAsync(Branch branch)
        {
            // Удалить связанные здания и комнаты (если нужно каскадное удаление)
            _dbContext.Branches.Remove(branch);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteRoomAsync(Guid roomId)
        {
            var room = await _dbContext.Rooms.FindAsync(roomId);
            if (room == null)
                throw new KeyNotFoundException("Room not found.");

            await DeleteRoomAsync(room);
        }

        public async Task DeleteRoomAsync(Room room)
        {
            _dbContext.Rooms.Remove(room);
            await _dbContext.SaveChangesAsync();
        }
    }
}
