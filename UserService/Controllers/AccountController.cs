using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Model.DataTransfer;
using Models;
using Models.DataTransfer;
using Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UserService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly Logic _logic;
        private readonly UserManager<ApplicationUser> _userManager;

        public AccountController(Logic logic, UserManager<ApplicationUser> userManager)
        {
            _logic = logic;
            _userManager = userManager;
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
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginDto userForAuthentication)
        {
            var user = await _userManager.FindByNameAsync(userForAuthentication.UserName);
            if (user == null || !await _userManager.CheckPasswordAsync(user, userForAuthentication.Password))
                return Unauthorized("Invalid Authentication");
            if (!await _userManager.IsEmailConfirmedAsync(user))
                return Unauthorized(new AuthResponseDto { ErrorMessage = "Email is not confirmed" });
            if (!await _userManager.CheckPasswordAsync(user, userForAuthentication.Password)) 
                return Unauthorized(new AuthResponseDto { ErrorMessage = "Invalid Authentication" });

            return Ok(await _logic.LoginUser(user));
        }


        [HttpGet("EmailConfirmation")]
        [AllowAnonymous]
        public async Task<IActionResult> EmailConfirmation([FromQuery] string email, [FromQuery] string token)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return BadRequest("Invalid Email Confirmation Request");
            var confirmResult = await _userManager.ConfirmEmailAsync(user, token);
            if (!confirmResult.Succeeded)
                return BadRequest("Invalid Email Confirmation Request");
            return Ok();
        }
    }
}
