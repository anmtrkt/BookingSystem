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
using BookingSystem.Infrastructure.Services.Interfaces;
using BookingSystem.Core.Domain.Models.BuildingModels;

namespace BookingSystem.API.Controllers.AdminController
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(Roles =Roles.Admin)]
    public class BuildingsController : ControllerBase
    {
        private readonly BookingSystemDbContext _context;
        private IBuildingService _buildingService;

        public BuildingsController(BookingSystemDbContext context, IBuildingService buildingService)
        {
            _context = context;
            _buildingService = buildingService;
        }

        // GET: api/Buildings
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Building>>> GetBuildings()
        {
            return Ok(await _buildingService.GetBuildingsAsync());
        }

        // GET: api/Buildings/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Building>> GetBuilding(Guid id)
        {
            var building = await _buildingService.GetBuildingAsync(id);

            if (building == null)
            {
                return NotFound();
            }

            return building;
        }

        // PUT: api/Buildings/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBuilding(Guid id, Building building)
        {
            if (id != building.Id)
            {
                return BadRequest();
            }

            await _buildingService.UpdateBuildingAsync(building);

            return NoContent();
        }

        // POST: api/Buildings
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Building>> PostBuilding([FromBody] CreateBuildingDto buildingdto)
        {
            var building = await _buildingService.CreateBuildingAsync(buildingdto);

            return CreatedAtAction("GetBuilding", new { id = building.Id }, building);
        }

        // DELETE: api/Buildings/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBuilding(Guid id)
        {
            var building = await _buildingService.GetBuildingAsync(id);
            if (building == null)
            {
                return NotFound();
            }

            await _buildingService.DeleteBuildingAsync(building.Id);

            return NoContent();
        }


    }
}
