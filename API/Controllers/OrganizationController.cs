using BookingSystem.Application.DTOs;
using BookingSystem.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookingSystem.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OrganizationController : ControllerBase
{
    private readonly IOrganizationService _orgService;

    public OrganizationController(IOrganizationService orgService)
    {
        _orgService = orgService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<OrganizationDto>>> GetAll()
    {
        return Ok(await _orgService.GetAllOrganizationsAsync());
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<OrganizationDto>> GetById(Guid id)
    {
      
            return Ok(await _orgService.GetOrganizationByIdAsync(id));
    
    }

    // Модификация - только Админ
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<OrganizationDto>> Create([FromBody] CreateOrganizationRequest request)
    {
        var org = await _orgService.CreateOrganizationAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = org.Id }, org);
    }

    [HttpPut]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<OrganizationDto>> Update([FromBody] UpdateOrganizationRequest request)
    {
  
            return Ok(await _orgService.UpdateOrganizationAsync(request));

    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _orgService.DeleteOrganizationAsync(id);
        return NoContent();
    }
}
