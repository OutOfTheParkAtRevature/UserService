﻿using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Model;
using Model.DataTransfer;
using Models;
using Models.DataTransfer;
using Newtonsoft.Json;
using Service;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;

namespace UserService
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin, League Manager, Head Coach, Assistant Coach, Parent, Player")]
    public class UserController : ControllerBase
    {
        private readonly Mapper _mapper;
        private readonly Logic _logic;

        public UserController(Mapper mapper, Logic logic)
        {
            _mapper = mapper;
            _logic = logic;
        }

        [HttpGet]
        [Authorize(Roles = "Admin, League Manager, Head Coach, Assistant Coach")]
        public async Task<IActionResult> GetUsers()
        {
            return Ok(await _logic.GetUsers());
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin, League Manager, Head Coach, Assistant Coach, Parent, Player")]
        public async Task<ActionResult<UserDto>> GetUser(string id)
        {
            ApplicationUser user = await _logic.GetUserById(id);
            if (user == null) return NotFound("No user with that ID found");
            return Ok(_mapper.ConvertUserToUserDto(user));
        }

        [HttpGet("username/{username}")]
        [Authorize(Roles = "Admin, League Manager, Head Coach, Assistant Coach, Parent, Player")]
        public async Task<ActionResult<UserDto>> GetUserByUsername(string username)
        {
            ApplicationUser user = await _logic.GetUserByUsername(username);
            if (user == null) return NotFound("No user with that username found");
            UserDto userDto = _mapper.ConvertUserToUserDto(user);

            var token = await HttpContext.GetTokenAsync("access_token");
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = await httpClient.GetAsync($"api/Team/{userDto.TeamID}");
                string apiResponse = await response.Content.ReadAsStringAsync();
                var team = JsonConvert.DeserializeObject<TeamDto>(apiResponse);
                userDto.Team = team;
            }

            return Ok(userDto);
        }

        [HttpGet("role/{id}")]
        [Authorize(Roles = "Admin, League Manager, Head Coach")]
        public async Task<IActionResult> GetUserRole(string id)
        {
            ApplicationUser user = await _logic.GetUserById(id);
            if (user == null) return NotFound("No user with that ID found");
            if (user.RoleName == null) return NoContent();
            return Ok(await _logic.GetUserRole(id));
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin, League Manager, Head Coach, Assistant Coach, Parent")]
        public async Task<ActionResult<UserDto>> EditUser(string id, [FromBody] EditUserDto editedUser)
        {
            var token = await HttpContext.GetTokenAsync("access_token");
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            string loggedInUserId = await _logic.GetLoggedInUserId(claimsIdentity);

            if (await _logic.AllowedToAlterUser(loggedInUserId, id) == false)
                return Forbid("Not authorized to edit this user.");

            return _mapper.ConvertUserToUserDto(await _logic.EditUser(id, editedUser));
        }

        [HttpPut("role/{id}")]
        [Authorize(Roles = "Admin, League Manager, Head Coach")]
        public async Task<IActionResult> ApproveUserRole(string id, [FromBody] EditUserDto eud)
        {
            var token = await HttpContext.GetTokenAsync("access_token");
            ApplicationUser user = await _logic.GetUserById(id);  
            if (user == null) return NotFound("No user with that ID found");
            return Ok(await _logic.AddUserRole(id, eud.RoleName, token));
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin, League Manager, Head Coach, Assistant Coach, Parent")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            string loggedInUserId = await _logic.GetLoggedInUserId(claimsIdentity);

            if (await _logic.AllowedToAlterUser(loggedInUserId, id) == false)
                return Forbid("Not authorized to delete this user.");

            bool result = await _logic.DeleteUser(id);

            if (result) return Ok("User deleted successfully");
            return NotFound("User not found");
        }

        [HttpGet("roles")]
        public async Task<IActionResult> GetRoles()
        {
            List<RoleDto> roles = await _logic.GetRoles();
            if (roles == null) return NotFound("No roles in db");
            return Ok(roles);
        }

        [HttpGet("roles/{id}")]
        public async Task<IActionResult> GetRoleById(string id)
        {
            RoleDto role = await _logic.GetRoleById(id);
            if (role == null) return NotFound("No role with that ID was found");
            return Ok(role);
        }
    }
}
