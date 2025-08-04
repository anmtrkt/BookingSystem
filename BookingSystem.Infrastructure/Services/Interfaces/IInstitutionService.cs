using BookingSystem.Core.Domain.Entities.Institutions;
using BookingSystem.Core.Domain.Models.InstitutionModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingSystem.Infrastructure.Services.Interfaces
{
    public interface IInstitutionService
    {
        public Task<Institution> CreateInstitutionAsync(CreateInstitutionDto institutionDto);
        public Task UpdateInstitutionAsync(Institution institution);
        public Task RemoveIsntitutionAsync(Institution institution);
        public Task<IEnumerable<Institution>> GetInstitutionsAsync();
        public Task<Institution> GetInstitutionAsync(Guid id);
        public Task<Institution> GetInstitutionByNameAsync(string name);
        public Task AddInstitutionAsync(Institution institution);
    }
}
