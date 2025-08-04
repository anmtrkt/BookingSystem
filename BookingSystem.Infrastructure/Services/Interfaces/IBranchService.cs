using BookingSystem.Core.Domain.Entities.Institutions;
using BookingSystem.Core.Domain.Models.BranchModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingSystem.Infrastructure.Services.Interfaces
{
    public interface IBranchService
    {
        public Task<IEnumerable<Branch>> GetBranchesAsync();
        public Task<Branch> CreateBranchAsync(CreateBranchDto createBranchDto);
        public Task<Branch> GetBranchAsync(Guid branchId);
        public Task<List<Room>> GetRoomsAsync(Guid branchId);
        public Task<List<Room>> GetRoomsAsync(Branch branch);
        public Task<List<Building>> GetBuildingsAsync(Guid branchId);
        public Task<List<Building>> GetBuildingsAsync(Branch branch);
        public Task UpdateAsync(Branch branch);
        public Task DeleteBranchAsync(Guid branchId);   
        public Task DeleteRoomAsync(Room room);
        public Task DeleteBranchAsync(Branch branch);
        
    }
}
