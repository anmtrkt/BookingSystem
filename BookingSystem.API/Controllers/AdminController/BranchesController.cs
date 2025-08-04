using BookingSystem.Core.Domain.Entities.Institutions;
using BookingSystem.Core.Domain.Models.BranchModel;
using BookingSystem.Infrastructure.Persistence;
using BookingSystem.Infrastructure.Services.Interfaces;
using BookingSystem.Infrastructure.Services.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookingSystem.API.Controllers.AdminController
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(Roles =Roles.Admin)]

    public class BranchesController : ControllerBase
    {

        private readonly IBranchService _branchService;
        private readonly BookingSystemDbContext _context;

        public BranchesController(BookingSystemDbContext context, IBranchService branchService)
        {
            _branchService = branchService;
            _context = context;
        }

        // GET: api/Branches
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Branch>>> GetBranches()
        {
            return Ok(await _branchService.GetBranchesAsync());
        }

        // GET: api/Branches/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Branch>> GetBranch(Guid id)
        {
            var branch = await _branchService.GetBranchAsync(id);

            if (branch == null)
            {
                return NotFound();
            }

            return branch;
        }

        // PUT: api/Branches/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBranch(Guid id, Branch branch)
        {
            if (id != branch.Id)
            {
                return BadRequest();
            }
            await _branchService.UpdateAsync(branch);


            return NoContent();
        }

        // POST: api/Branches
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Branch>> PostBranch([FromBody]CreateBranchDto branchdto)
        {
            var branch = await _branchService.CreateBranchAsync(branchdto);
           
            return CreatedAtAction("GetBranch", new { id = branch.Id }, branch);
        }

        // DELETE: api/Branches/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBranch(Guid id)
        {
            var branch = await _branchService.GetBranchAsync(id);
            if (branch == null)
            {
                return NotFound();
            }
            await _branchService.DeleteBranchAsync(id);

            return NoContent();
        }


    }
}
