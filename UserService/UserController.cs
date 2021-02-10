using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Model.DataTransfer;
using Models;
using Models.DataTransfer;
using Service;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;

namespace UserService
{
    [Route("api/[controller]")]
    [ApiController]
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
        public async Task<IActionResult> Create([FromBody] CreateUserDto cud)
        {
            var userExists = await _userManager.FindByNameAsync(cud.UserName);
            if (userExists != null)
                return Conflict("User already exists!");

            return Ok(await _logic.CreateUser(cud));
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto userForAuthentication)
        {
            var user = await _userManager.FindByNameAsync(userForAuthentication.UserName);
            if (user == null || !await _userManager.CheckPasswordAsync(user, userForAuthentication.Password))
                return Unauthorized(new AuthResponseDto { IsAuthSuccessful = false, ErrorMessage = "Invalid Authentication" }) ;

            return Ok(await _logic.LoginUser(user));
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            return Ok(await _logic.GetUsers());
        }

        /// <summary>
        /// Get a user by their username
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        //[HttpGet("{username}")]
        //public async Task<IActionResult> GetUserByUsername(string username)
        //{
        //    return _mapper.ConvertUserToUserDto(await _logic.GetUserByUsername(username));
        //}

        [HttpGet("{id}")]
        public async Task<ActionResult<UserDto>> GetUser(string id)
        {
            return _mapper.ConvertUserToUserDto(await _logic.GetUserById(id));
        }

        /// <summary>
        /// Get specified User's Role
        /// </summary>
        /// <param name="id"></param>
        /// <param name="editedUser"></param>
        /// <returns></returns>
        //[HttpGet("{id}/Role")]
        //public async Task<IActionResult> GetUserRole(string id)
        //{
        //    return Ok(await _logic.GetUserRole(id));
        //}


        [HttpPut("edit/{id}")]
        public async Task<ActionResult<ApplicationUser>> EditUser(string id, EditUserDto editedUser)
        {
            return await _logic.EditUser(id, editedUser);
        }


        [HttpDelete("delete/{id}")]
        public async Task<ApplicationUser> DeleteUser(string id)
        {
            _logger.LogInformation("User deleted.");
            return await _logic.DeleteUser(id);
        }
    }
}
