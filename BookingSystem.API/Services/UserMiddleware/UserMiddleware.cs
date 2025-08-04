using BookingSystem.API.Services.Extensions;
using BookingSystem.API.Services.Identity;
using BookingSystem.API.Services.Models;
using BookingSystem.Core.Domain.Entities.Users;
using BookingSystem.Infrastructure.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;


namespace BookingSystem.API.Services.UserServices
{
    public class UserMiddleware : IUserMiddleware
    {

        private readonly IHttpContextAccessor _httpContextAccessor;

        private readonly IInstitutionService _institutionService;
        private readonly IUserService _userService;
        private readonly ITokenService _token;
        private readonly IConfiguration _configuration;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Roles> _roleManager;

        public UserMiddleware(RoleManager<Roles> roleManager, IInstitutionService institutionService, IUserService userService, ITokenService token, IConfiguration configuration, UserManager<User> userManager, IHttpContextAccessor httpContextAccessor)
        {
            _institutionService = institutionService;
            _userService = userService; 
            _roleManager = roleManager;
            _httpContextAccessor = httpContextAccessor;
            _token = token;
            _configuration = configuration;
            _userManager = userManager;
        }
        public async Task<object?> Registration(RegisterRequest request)
        {
            var inst = await _institutionService.GetInstitutionByNameAsync(request.Institution);
            var user = User.Create(
                request.Name,
                request.Surname,
                request.Patronymic,
                request.PhoneNumber,
                request.Email,
                inst,
                request.Post);

            var result = await _userManager.CreateAsync(user, request.Password);

            if (!result.Succeeded) return null;

            var findUser =
                await _userManager.FindByEmailAsync(request.Email)
                ??
                throw new Exception($"User {request.Email} not found");

            await _userManager.AddToRoleAsync(findUser, Roles.User);
            return new AuthenticationRequest
            {
                Email = request.Email,
                Password = request.Password
            };
        }
        public async Task<AuthResponse> Authenticate(User user)
        {
            var httpContext = _httpContextAccessor.HttpContext;
            var roles = await _userManager.GetRolesAsync(user);
/*            var roleIds = await _roleManager.FindByNameAsync();*/


            var accessToken = _token.CreateToken(user, roles);
            user.RefreshToken = _configuration.GenerateRefreshToken();
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(_configuration.GetSection("Jwt:RefreshTokenValidityInDays").Get<int>());

            return new AuthResponse
            {
                Username = user.UserName!,
                Email = user.Email!,
                Token = accessToken,
                RefreshToken = user.RefreshToken
            };
        }
    }
}
