using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Model;
using Model.DataTransfer;
using Models;
using Models.DataTransfer;
using Service;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace UserService
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin, League Manager, Head Coach, Assistant Coach, Parent, Player")]
    public class UserController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly Mapper _mapper;
        private readonly JwtHandler _jwtHandler;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly Logic _logic;
        private readonly ILogger<UserController> _logger;

        public UserController(UserManager<ApplicationUser> userManager, Mapper mapper, JwtHandler jwtHandler, RoleManager<IdentityRole> roleManager, Logic logic, ILogger<UserController> logger)
        {
            _userManager = userManager;
            _mapper = mapper;
            _jwtHandler = jwtHandler;
            _roleManager = roleManager;
            _logic = logic;
            _logger = logger;
        }


        [HttpPost("create")]
        [AllowAnonymous]
        public async Task<IActionResult> Create([FromBody] CreateUserDto cud)
        {
            var userExists = await _userManager.FindByNameAsync(cud.UserName);
            if (userExists != null)
                return Conflict("User already exists!");

            return Ok(await _logic.CreateUser(cud));
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginDto userForAuthentication)
        {
            var user = await _userManager.FindByNameAsync(userForAuthentication.UserName);
            if (user == null || !await _userManager.CheckPasswordAsync(user, userForAuthentication.Password))
                return Unauthorized("Invalid Authentication");

            return Ok(await _logic.LoginUser(user));
        }

        [HttpGet]
        [Authorize(Roles = "Admin, League Manager, Head Coach, Assistant Coach")]
        public async Task<IActionResult> GetUsers()
        {
            return Ok(await _logic.GetUsers());
        }

        [HttpGet("{username}")]
        public async Task<ActionResult<UserDto>> GetUserByUsername(string username)
        {
            return _mapper.ConvertUserToUserDto(await _logic.GeUserByUsername(username));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserDto>> GetUser(string id)
        {
            return _mapper.ConvertUserToUserDto(await _logic.GetUserById(id));
        }

        [HttpGet("{id}/Role")]
        [Authorize]
        public async Task<IActionResult> GetUserRole(string id)
        {
            return Ok(await _logic.GetUserRole(id));
        }


        [HttpPut("edit/{id}")]
        [Authorize]
        public async Task<ActionResult<UserDto>> EditUser(string id, [FromBody] EditUserDto editedUser)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            string loggedInUserId = await _logic.GetLoggedInUserId(claimsIdentity);

            if (loggedInUserId != id && await _logic.AllowedToAlterUser(loggedInUserId, id) == false)
                return Forbid("Not authorized to edit this user.");

            return _mapper.ConvertUserToUserDto(await _logic.EditUser(id, editedUser));
        }


        [HttpDelete("delete/{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            string loggedInUserId = await _logic.GetLoggedInUserId(claimsIdentity);

            if (loggedInUserId != id && await _logic.AllowedToAlterUser(loggedInUserId, id) == false)
                return Forbid("Not authorized to delete this user.");

            bool result = await _logic.DeleteUser(id);

            if (result) return Ok("User deleted successfully");
            return NotFound("User not found");
        }
    }
}
