using BookingSystem.Core.Domain.Entities.Institutions;
using BookingSystem.Core.Domain.Models.InstitutionModels;
using BookingSystem.Infrastructure.Persistence;
using BookingSystem.Infrastructure.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using OpenTelemetry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingSystem.Infrastructure.Services.Services
{
    public class InstitutionService : IInstitutionService
    {
        private readonly BookingSystemDbContext _dbContext;
        public InstitutionService(BookingSystemDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<Institution> CreateInstitutionAsync(CreateInstitutionDto institutionDto)
        {
            var parent = _dbContext.Institutions.Find(institutionDto.ParentId);
            var inst = Institution.Create(institutionDto.Name, institutionDto.Priority, parent);
            await _dbContext.Institutions.AddAsync(inst);
            await _dbContext.SaveChangesAsync();
            return inst;
        }
        public async Task UpdateInstitutionAsync(Institution institution)
        {

            _dbContext.Entry(institution).State = EntityState.Modified;
             await _dbContext.SaveChangesAsync();
        }
        public async Task RemoveIsntitutionAsync(Institution institution)
        {
            _dbContext.Institutions.Remove(institution);
             await _dbContext.SaveChangesAsync();
        }
        public async Task<IEnumerable<Institution>> GetInstitutionsAsync() {
            return await _dbContext.Institutions.ToListAsync();
        }

        public async Task AddInstitutionAsync(Institution institution)
        {
            _dbContext.Institutions.Add(institution);
            await _dbContext.SaveChangesAsync();
        }
        public async Task<Institution> GetInstitutionAsync(Guid id)
        {
            var inst = await _dbContext.Institutions.FindAsync(id);
            if (inst == null) throw new KeyNotFoundException($"there is no such a Institutuion {id}");
            return inst;
        }
        public async Task<Institution> GetInstitutionByNameAsync(string name)
        {
            var inst = await _dbContext.Institutions.FirstOrDefaultAsync(i => i.NormalizedName == name.ToUpper());
            if (null == inst) throw new KeyNotFoundException($"there is no such a Institution {name.ToUpper()}");
            return inst;
        }
    }
}
