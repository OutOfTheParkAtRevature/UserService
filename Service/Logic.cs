using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Model;
using Model.DataTransfer;
using Models;
using Models.DataTransfer;
using Newtonsoft.Json;
using Repository;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class Logic
    {
        private readonly Repo _repo;
        private readonly Mapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly JwtHandler _jwtHandler;
        private readonly ILogger<Repo> _logger;
        private readonly HttpClient _httpClient;

        public Logic(Repo repo, UserManager<ApplicationUser> userManager, Mapper mapper, JwtHandler jwtHandler, ILogger<Repo> logger, RoleManager<IdentityRole> roleManager)
        {
            _repo = repo;
            _mapper = mapper;
            _logger = logger;
            _jwtHandler = jwtHandler;
            _roleManager = roleManager;
            _userManager = userManager;
        }

        /// <summary>
        /// Takes user input, creates authentication data
        /// </summary>
        /// <param name="User">User info sent from controller</param>
        /// <returns>UserLoggedInDto</returns>
        /// 
        public async Task<UserLoggedInDto> LoginUser(ApplicationUser user)
        {
            var signingCredentials = _jwtHandler.GetSigningCredentials();
            var claims = _jwtHandler.GetClaims(user);
            var userRoles = await _userManager.GetRolesAsync(user);
            foreach (var userRole in userRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, userRole));
            }
            var tokenOptions = _jwtHandler.GenerateTokenOptions(signingCredentials, claims);
            var token = new JwtSecurityTokenHandler().WriteToken(tokenOptions);
            UserLoggedInDto uld = _mapper.ConvertUserToUserLoggedInDto(user);
            uld.Token = token;
            return uld;
        }


        /// <summary>
        /// Takes CreateUserDto from controller, creates a user, creates roles if they don't exists,
        /// adds user to Player role, sends a notification to an admin user to approve their originally requested role and 
        /// returns a UserLoggedInDto
        /// </summary>
        /// <param name="cud"></param>
        /// <returns>UserLoggedInDto</returns>
        public async Task<AuthResponseDto> CreateUser(CreateUserDto cud)
        {
            if (!await _roleManager.RoleExistsAsync(Roles.A))
            {
                await _roleManager.CreateAsync(new IdentityRole(Roles.A));
                await _roleManager.CreateAsync(new IdentityRole(Roles.LM));
                await _roleManager.CreateAsync(new IdentityRole(Roles.HC));
                await _roleManager.CreateAsync(new IdentityRole(Roles.AC));
                await _roleManager.CreateAsync(new IdentityRole(Roles.PT));
                await _roleManager.CreateAsync(new IdentityRole(Roles.PL));
            }

            ApplicationUser user = new ApplicationUser
            {
                FullName = cud.FullName,
                PhoneNumber = cud.PhoneNumber,
                Email = cud.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = cud.UserName,
                RoleName = cud.RoleName,
                TeamID = cud.TeamID,
            };
            var result = await _userManager.CreateAsync(user, cud.Password);
            if (!result.Succeeded)
            {
                return new AuthResponseDto { IsAuthSuccessful = false, ErrorMessage = result.Errors.ToString() };
            }
            //if (user.RoleName == "Parent")
            //{

            //}
            await _userManager.AddToRoleAsync(user, Roles.PL);

            //Create notification for Head Coach/League Manager with user info and requested role
            //allow them to set Role accordingly

            return new AuthResponseDto { IsAuthSuccessful = true };
        }

        /// <summary>
        /// UserID -> Repo.GetUser
        /// </summary>
        /// <param name="id">user id</param>
        /// <returns>user</returns>
        public async Task<ApplicationUser> GetUserById(string id)
        {
            return await _repo.GetUserById(id);
        }

        /// <summary>
        /// Gets the user with the specified username
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public async Task<ApplicationUser> GeUserByUsername(string username)
        {
            return await _repo.GetUserByUsername(username);
        }

        /// <summary>
        /// Gets the role of a user with the specified user Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<string> GetUserRole(string id)
        {
            ApplicationUser user = await GetUserById(id);
            return user.RoleName;
        }


        /// <summary>
        /// Get the logged in User Id
        /// </summary>
        /// <param name="claimsIdentity"></param>
        /// <returns></returns>
        public async Task<string> GetLoggedInUserId(ClaimsIdentity claimsIdentity)
        {
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            ApplicationUser user = await GeUserByUsername(claim.Value);
            return user.Id;
        }

        /// <summary>
        /// Gets list of Users 
        /// </summary>
        /// <returns>List<UserDto></UserDto></returns>
        public async Task<IEnumerable<UserDto>> GetUsers()
        {
            IEnumerable<ApplicationUser> users = await _repo.GetUsers();
            List<UserDto> userDtos = new List<UserDto>();
            foreach (var user in users)
            {
                UserDto userDto = _mapper.ConvertUserToUserDto(user);
                userDtos.Add(userDto);
            }
            return userDtos;
        }

        

        
        /// <summary>
        /// Checks if user or email already exists in DB
        /// </summary>
        /// <param name="username"></param>
        /// <param name="email"></param>
        /// <returns>Boolean</returns>
        public async Task<bool> UserExists(string username, string email)
        {
            // should be && so that username and email are both unique right?
            bool userExists = await _repo.Users.AnyAsync(x => x.UserName == username && x.Email == email);
            if (userExists)
            {
                _logger.LogInformation("User found in database");
                return userExists;
            }
            return userExists;
        }
        
        
        /// <summary>
        /// Delete user from context by ID and return true if user deleted, false if not
        /// </summary>
        /// <param name="id">UserID</param>
        /// <returns>bool</returns>
        public async Task<bool> DeleteUser(string id)
        {
            ApplicationUser user = await GetUserById(id);
            if (user != null)
            {
                _repo.Users.Remove(user);
                await _repo.CommitSave();
                _logger.LogInformation("User removed");
                return true;
            }
            else
            {
                _logger.LogInformation("User not found");
                return false;
            }
        }

        /// <summary>
        /// Add user Role to User by ID
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="roleId"></param>
        /// <returns>modified User</returns>
        public async Task<ApplicationUser> AddUserRole(string userId, string RoleName)
        {
            ApplicationUser tUser = await GetUserById(userId);
            await _userManager.RemoveFromRoleAsync(tUser, tUser.RoleName);
            tUser.RoleName = RoleName;
            await _userManager.AddToRoleAsync(tUser, RoleName);
            Guid carpoolId;
            if (RoleName == "Parent")
            {
                using (var httpClient = new HttpClient())
                {
                    using (var response = await httpClient.GetAsync($"api/League/Team/{tUser.TeamID}"))
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();

                        var team = JsonConvert.DeserializeObject<TeamDto>(apiResponse);
                        carpoolId = team.CarpoolID;
                    }
                }
                RecipientListDto rLD = new RecipientListDto()
                {
                    RecipientListID = carpoolId,
                    RecipientID = userId
                };
                using (var httpClient = new HttpClient())
                {
                    var response = await httpClient.PostAsJsonAsync($"api/Message/RecipientLists/Create", rLD);
                }
            }
            await _repo.CommitSave();
            return tUser;
        }

        /// <summary>
        /// Checks if input data is different from existing and updates if so
        /// </summary>
        /// <param name="userId">User to edit</param>
        /// <param name="editUserDto">New information</param>
        /// <returns>modified User</returns>
        public async Task<ApplicationUser> EditUser(string userId, EditUserDto editUserDto)
        {
            ApplicationUser tUser = await GetUserById(userId);

            if (tUser.FullName != editUserDto.FullName && editUserDto.FullName != "") { tUser.FullName = editUserDto.FullName; }
            if (tUser.Email != editUserDto.Email && !string.IsNullOrEmpty(editUserDto.Email)) { tUser.Email = editUserDto.Email; tUser.NormalizedEmail = editUserDto.Email.ToUpper(); }
            if (tUser.PhoneNumber != editUserDto.PhoneNumber && editUserDto.PhoneNumber != "") { tUser.PhoneNumber = editUserDto.PhoneNumber; }
            if (!string.IsNullOrEmpty(editUserDto.OldPassword) && !string.IsNullOrEmpty(editUserDto.NewPassword)) { await _userManager.ChangePasswordAsync(tUser, editUserDto.OldPassword, editUserDto.NewPassword); }
            if (tUser.RoleName != editUserDto.RoleName && editUserDto.RoleName != "") { 
                await _userManager.RemoveFromRoleAsync(tUser, tUser.RoleName); 
                await _userManager.AddToRoleAsync(tUser, editUserDto.RoleName);
                tUser.RoleName = editUserDto.RoleName;
            }
            if (tUser.TeamID != editUserDto.TeamID/* && editUserDto.TeamID != null*/) { tUser.TeamID = editUserDto.TeamID; }

            await _repo.CommitSave();
            return tUser;
        }


        /// <summary>
        /// Checks to see if the logged in user is allowed to alter a given user:
        /// Admins can alter all users besides other Admins.
        /// League Managers can alter all users besides Admins and other League Managers.
        /// Head Coaches can alter Assitant Coaches, Parents and Players.
        /// Assistant Coaches, Parents and Players can't alter other users.
        /// </summary>
        /// <param name="loggedInUserId"></param>
        /// <param name="userToEditId"></param>
        /// <returns></returns>
        public async Task<bool> AllowedToAlterUser(string loggedInUserId, string userToEditId)
        {
            ApplicationUser loggedInUser = await GetUserById(loggedInUserId);
            ApplicationUser userToEdit = await GetUserById(userToEditId);
            switch(loggedInUser.RoleName){
                case Roles.A: if (userToEdit.RoleName != Roles.A) return true;
                                return false;
                case Roles.LM: if(userToEdit.RoleName != Roles.A || userToEdit.RoleName != Roles.LM ) return true;
                                return false;
                case Roles.HC: if (userToEdit.RoleName != Roles.A || userToEdit.RoleName != Roles.LM || userToEdit.RoleName != Roles.HC) return true;
                                return false;
                default: return false;
            }


        }
        
    }
}
