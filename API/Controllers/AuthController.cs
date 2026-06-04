using BookingSystem.Api.Services;
using BookingSystem.Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;

namespace BookingSystem.Api;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserManager<AppUser> _userManager;
    private readonly TokenService _tokenService;

    public AuthController(UserManager<AppUser> userManager, TokenService tokenService)
    {
        _userManager = userManager;
        _tokenService = tokenService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        // 1. Ищем пользователя
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null) return Unauthorized("Пользователь не найден.");

        // 2. Проверяем пароль
        var result = await _userManager.CheckPasswordAsync(user, request.Password);
        if (!result) return Unauthorized("Неверный пароль.");

        // 3. Получаем роли и генерируем токен
        var roles = await _userManager.GetRolesAsync(user);
        var token = _tokenService.GenerateToken(user, roles);

        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddHours(1)
        };

        Response.Cookies.Append("auth_token", token, cookieOptions);

        return Ok(new { message = "OK" });
    }
}
