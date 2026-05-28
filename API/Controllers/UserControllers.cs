using BookingSystem.Application.DTOs;
using BookingSystem.Application.Services;
using BookingSystem.Core.Entities; // Для Enum Role
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BookingSystem.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")] // ВЕСЬ контроллер доступен только Админу
public class UserController : ControllerBase
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IUserService _userService;

    public UserController(UserManager<AppUser> userManager, IUserService userService)
    {
        _userManager = userManager;
        _userService = userService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request)
    {
        // Проверяем, существует ли пользователь
        var existingUser = await _userManager.FindByEmailAsync(request.Email);
        if (existingUser != null) return BadRequest("Пользователь с таким Email уже существует.");
        var user = await _userService.CreateUserAsync(request);



        //await _userManager.AddToRoleAsync(user);
        await _userManager.AddToRoleAsync(user, "User");


        return Ok(new { Message = "Пользователь создан", UserId = user.Id });
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetAll()
    {
        var users = await _userService.GetAllUsersAsync();
        return Ok(users);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<UserDto>> GetById(Guid id)
    {
        
            var user = await _userService.GetUserByIdAsync(id);
            return Ok(user);
     
    }


    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());
        if (user == null) return NotFound();

        await _userManager.DeleteAsync(user);
        return NoContent();
    }
}
