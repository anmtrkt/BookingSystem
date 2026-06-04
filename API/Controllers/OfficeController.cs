using BookingSystem.Application.DTOs;
using BookingSystem.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookingSystem.Api;


[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OfficeController : ControllerBase
{
    private readonly IOfficeService _officeService;

    public OfficeController(IOfficeService officeService)
    {
        _officeService = officeService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<OfficeDto>>> GetAll()
    {
        return Ok(await _officeService.GetAllOfficesAsync());
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<OfficeDto>> GetById(Guid id)
    {
        return Ok(await _officeService.GetOfficeByIdAsync(id)); 
        
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<OfficeDto>> Create([FromBody] CreateOfficeRequest request)
    {
        var office = await _officeService.CreateOfficeAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = office.Id }, office);
    }

    [HttpPut]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<OfficeDto>> Update([FromBody] UpdateOfficeRequest request)
    {
         return Ok(await _officeService.UpdateOfficeAsync(request)); 
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _officeService.DeleteOfficeAsync(id);
        return NoContent();
    }
}
