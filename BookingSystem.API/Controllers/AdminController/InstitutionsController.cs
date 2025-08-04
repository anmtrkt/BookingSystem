using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookingSystem.Core.Domain.Entities.Institutions;
using BookingSystem.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using BookingSystem.Core.Domain.Entities.Users;
using BookingSystem.Infrastructure.Services.Interfaces;
using BookingSystem.Core.Domain.Models.InstitutionModels;

namespace BookingSystem.API.Controllers.AdminController
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(Roles = Roles.Admin)]
    
    public class InstitutionsController : ControllerBase
    {

        private readonly IInstitutionService _institutionService;
        private readonly BookingSystemDbContext _context;

        public InstitutionsController(BookingSystemDbContext context, IInstitutionService institutionService)
        {
            _institutionService = institutionService;
            _context = context;
        }

        // GET: api/Institutions
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Institution>>> GetInstitutions()
        {
            return Ok(await _institutionService.GetInstitutionsAsync());
        }

        // GET: api/Institutions/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Institution>> GetInstitution(Guid id)
        {
            var institution = await _institutionService.GetInstitutionAsync(id);

            if (institution == null)
            {
                return NotFound();
            }

            return institution;
        }

        // PUT: api/Institutions/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutInstitution(Guid id, Institution institution)
        {
            if (id != institution.Id)
            {
                return BadRequest();
            }
            await _institutionService.UpdateInstitutionAsync(institution);

            return NoContent();
        }

        // POST: api/Institutions
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Institution>> PostInstitution([FromBody] CreateInstitutionDto institutiondto)
        {

            var inst = await _institutionService.CreateInstitutionAsync(institutiondto);

            return CreatedAtAction("GetInstitution", new { id = inst.Id }, inst);
        }

        // DELETE: api/Institutions/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteInstitution(Guid id)
        {
            var institution = await _institutionService.GetInstitutionAsync(id);
            if (institution == null)
            {
                return NotFound();
            }

            await _institutionService.RemoveIsntitutionAsync(institution);
     

            return NoContent();
        }
    }
}
