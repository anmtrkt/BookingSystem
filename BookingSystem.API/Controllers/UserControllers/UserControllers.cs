using BookingSystem.API.Services.Models;
using BookingSystem.API.Services.UserServices;
using BookingSystem.Core.Domain.Entities.Users;
using Microsoft.AspNetCore.JsonPatch;
using BookingSystem.Infrastructure.Services.Interfaces;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Threading.Tasks;
using BookingSystem.Core.Domain.Entities.Aggregates;
using BookingSystem.Core.Domain.Models.UserModels;
using MassTransit.Serialization;

namespace BookingSystem.API.Controllers.UserControllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IUserMiddleware _userMiddleware;
        private readonly IEmailService _emailService;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Roles> _roleManager;
        private readonly ILogger<UsersController> _logger;

        public UsersController(ILogger<UsersController> logger, IEmailService emailService, IUserService userService, IUserMiddleware userMiddleware, UserManager<User> userManager, RoleManager<Roles> roleManager)
        {
            _emailService = emailService;
            _userManager = userManager;
            _userService = userService;
            _userMiddleware = userMiddleware;
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
        }
        [HttpPost("auth")]
        [ProducesResponseType(typeof(AuthResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Authenticate(AuthenticationRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var managedUser = await _userManager.FindByEmailAsync(request.Email);
            if (managedUser == null)
            {
                return BadRequest("Bad credentials");
            }

            var isPasswordValid = await _userManager.CheckPasswordAsync(managedUser, request.Password);
            if (!isPasswordValid)
            {
                return BadRequest("Bad credentials");
            }

            var user = await _userManager.FindByEmailAsync(request.Email);

            if (user is null)
                return Unauthorized();


            var temp = await _userMiddleware.Authenticate(user);
            HttpContext.Response.Cookies.Append("test", temp.Token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                Expires = DateTime.UtcNow.AddMinutes(30)
            });
            return Ok(temp);
        }
        [HttpGet("ManagedUser")]
        [Authorize(Roles = Roles.Manager)]
        [ProducesResponseType(typeof(List<UserDto>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetManagedUsers()
        {
            var currentUser = _userManager.GetUserAsync(User).Result;
            if (currentUser == null) { return BadRequest(); }
            if (!currentUser.IsManager) { return Unauthorized(); }
            return Ok(await _userService.GetManagedUsersAsync(currentUser));
        }



        [HttpPost("registration")]
        [ProducesResponseType(typeof(AuthResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Register(RegisterRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(request);

            var registrationRequest = await _userMiddleware.Registration(request);



            if (registrationRequest is null)
                return BadRequest(request);
            else if (registrationRequest is AuthenticationRequest ar)
            {
                await _emailService.RegisterMail(request.Email, request.Name + " " + request.Patronymic);
                return Ok(await Authenticate(ar));
            }

            return BadRequest(request);
        }
        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(Guid id)
        {
            var user = await _userService.GetUserAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }
/*        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPut("{id}")]
        public async Task<IActionResult> PatchUser(Guid id,
        [FromBody] UserUpdateDto updateDto)
        {
         
            var user = await _userService.GetUserAsync(id);
            if (user == null) return NotFound();
            _userService.UpdateFromDto(updateDto);
           

            // 3. Проверьте ModelState и обновите в БД
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            _userService.UpdateAsync(user);
            return NoContent(); // 204
        }*/

        /*namespace UserApi.Controllers
        {

                [HttpGet]
                public ActionResult<IEnumerable<User>> GetUsers()
                {
                    return Ok(users);
                }

                // GET: api/users/{id}
                [HttpGet("{id}")]
                public ActionResult<User> GetUser(int id)
                {
                    var user = users.FirstOrDefault(u => u.Id == id);
                    if (user == null)
                    {
                        return NotFound();
                    }
                    return Ok(user);
                }
            }*/
    }
}
