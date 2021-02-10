using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Model;
using Model.DataTransfer;
using Models;
using Models.DataTransfer;
using Repository;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
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
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly JwtHandler _jwtHandler;
        private readonly ILogger<Repo> _logger;

        public Logic(Repo repo, UserManager<User> userManager, Mapper mapper, JwtHandler jwtHandler, ILogger<Repo> logger, RoleManager<IdentityRole> roleManager)
        {
            _repo = repo;
            _mapper = mapper;
            _logger = logger;
            _jwtHandler = jwtHandler;
            _roleManager = roleManager;
        }

        /// <summary>
        /// Takes user input, creates authentication data
        /// </summary>
        /// <param name="User">User info sent from controller</param>
        /// <returns>AuthResponseDto</returns>
        /// 
        public async Task<AuthResponseDto> LoginUser(User user)
        {
            var signingCredentials = _jwtHandler.GetSigningCredentials();
            var claims = _jwtHandler.GetClaims(user);

            claims.Add(new Claim(ClaimTypes.Name, user.UserName));

            var userRoles = await _userManager.GetRolesAsync(user);
            foreach (var userRole in userRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, userRole));
            }

            var tokenOptions = _jwtHandler.GenerateTokenOptions(signingCredentials, claims);
            var token = new JwtSecurityTokenHandler().WriteToken(tokenOptions);
            return new AuthResponseDto { IsAuthSuccessful = true, Token = token };
        }
        
        /// <summary>
        /// UserID -> Repo.GetUser
        /// </summary>
        /// <param name="id">user id</param>
        /// <returns>user</returns>
        public async Task<User> GetUserById(string id)
        {
            return await _repo.GetUserById(id);
        }

        /// <summary>
        /// Gets list of Users 
        /// </summary>
        /// <returns>List<UserDto></UserDto></returns>
        public async Task<IEnumerable<UserDto>> GetUsers()
        {
            IEnumerable<User> users = await _repo.GetUsers();
            List<UserDto> userDtos = new List<UserDto>();
            foreach (var user in users)
            {
                UserDto userDto = _mapper.ConvertUserToUserDto(user);
                userDtos.Add(userDto);
            }
            return userDtos;
        }

        /// <summary>
        /// Takes CreateUserDto from controller, creates a user, creates roles if they don't exists,
        /// adds user to the specified role and returns a UserLoggedInDto
        /// </summary>
        /// <param name="cud"></param>
        /// <returns>UserLoggedInDto</returns>
        public async Task<UserLoggedInDto> CreateUser(CreateUserDto cud)
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

            User user = new User
            {
                FullName = cud.FullName,
                PhoneNumber = cud.PhoneNumber,
                Email = cud.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = cud.UserName,
                RoleID = cud.RoleID,
                TeamID = cud.TeamID,
            };
            await _userManager.CreateAsync(user, cud.Password);
            string role = _mapper.RoleIDtoRole(cud.RoleID);

            await _userManager.AddToRoleAsync(user, role);

            return _mapper.ConvertUserToUserLoggedInDto(user);
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
        /// Fetches user from context
        /// </summary>
        /// <param name="loginDto">User to search for</param>
        /// <returns>User</returns>
        //public async Task<User> LoginUser(UserForAuthenticationDto userForAuthentication)
        //{
        //    var user = await _userManager.FindByNameAsync(userForAuthentication.Email);
        //    if (user == null || !await _userManager.CheckPasswordAsync(user, userForAuthentication.Password))
        //        return Unauthorized(new AuthResponseDto { ErrorMessage = "Invalid Authentication" });
        //    var signingCredentials = _jwtHandler.GetSigningCredentials();
        //    var claims = _jwtHandler.GetClaims(user);
        //    var tokenOptions = _jwtHandler.GenerateTokenOptions(signingCredentials, claims);
        //    var token = new JwtSecurityTokenHandler().WriteToken(tokenOptions);
        //    return Ok(new AuthResponseDto { IsAuthSuccessful = true, Token = token });
        //    return await _repo.Users.SingleOrDefaultAsync(x => x.UserName == loginDto.UserName);
        //}
        /// <summary>
        /// Verifies password passed from user input
        /// </summary>
        /// <param name="user"></param>
        /// <param name="loginDto"></param>
        /// <returns>UserLoggedInDto</returns>
        //public async Task<UserLoggedInDto> CheckPassword(Task<User> user, LoginDto loginDto)
        //{
        //    using var hmac = new HMACSHA512(user.Result.PasswordSalt);
        //    var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));
        //    for (int i = 0; i < computedHash.Length; i++)
        //    {
        //        if (computedHash[i] != user.Result.PasswordHash[i])
        //        {
        //            return null;
        //        }
        //    }
        //    User loggedIn = await user;
        //    UserLoggedInDto loggedInUser = _mapper.ConvertUserToUserLoggedInDto(loggedIn);
        //    loggedInUser.Token = _token.CreateToken(loggedIn);
        //    return loggedInUser;
        //}
        /// <summary>
        /// Delete user from context by ID
        /// </summary>
        /// <param name="id">UserID</param>
        /// <returns>deleted User</returns>
        public async Task<User> DeleteUser(string id)
        {
            User user = await GetUserById(id);
            if (user != null)
            {
                _repo.Users.Remove(user);
                await _repo.CommitSave();
                _logger.LogInformation("User removed");
            }
            else
            {
                _logger.LogInformation("User not found");
            }
            return user;
        }
        /// <summary>
        /// Add user Role to User by ID
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="roleId"></param>
        /// <returns>modified User</returns>
        public async Task<User> AddUserRole(string userId, int roleId)
        {
            User tUser = await GetUserById(userId);
            tUser.RoleID = roleId;
            await _repo.CommitSave();
            return tUser;
        }
        /// <summary>
        /// Checks if input data is different from existing and updates if so
        /// </summary>
        /// <param name="userId">User to edit</param>
        /// <param name="editUserDto">New information</param>
        /// <returns>modified User</returns>
        public async Task<User> EditUser(string userId, EditUserDto editUserDto)
        {
            User tUser = await GetUserById(userId);

            if (tUser.FullName != editUserDto.FullName && editUserDto.FullName != "") { tUser.FullName = editUserDto.FullName; }
            if (tUser.Email != editUserDto.Email && editUserDto.Email != "") { tUser.Email = editUserDto.Email; tUser.NormalizedEmail = editUserDto.Email.ToUpper(); }
            if (tUser.PhoneNumber != editUserDto.PhoneNumber && editUserDto.PhoneNumber != "") { tUser.PhoneNumber = editUserDto.PhoneNumber; }
            if (!string.IsNullOrEmpty(editUserDto.OldPassword) && !string.IsNullOrEmpty(editUserDto.NewPassword)) { await _userManager.ChangePasswordAsync(tUser, editUserDto.OldPassword, editUserDto.NewPassword); }

            await _repo.CommitSave();
            return tUser;
        }
        /// <summary>
        /// Same as above, more options for higher user level
        /// </summary>
        /// <param name="userId">User to edit</param>
        /// <param name="coachEditUserDto">New information</param>
        /// <returns>modified User</returns>
        public async Task<User> CoachEditUser(string userId, CoachEditUserDto coachEditUserDto)
        {
            User tUser = await GetUserById(userId);
            if (tUser.FullName != coachEditUserDto.FullName && coachEditUserDto.FullName != "") { tUser.FullName = coachEditUserDto.FullName; }
            if (tUser.Email != coachEditUserDto.Email && coachEditUserDto.Email != "") { tUser.Email = coachEditUserDto.Email; }
            if (tUser.Password != coachEditUserDto.Password && coachEditUserDto.Password != "") { tUser.Password = coachEditUserDto.Password; }
            if (tUser.PhoneNumber != coachEditUserDto.PhoneNumber && coachEditUserDto.PhoneNumber != "") { tUser.PhoneNumber = coachEditUserDto.PhoneNumber; }
            if (tUser.RoleID != coachEditUserDto.RoleID && coachEditUserDto.RoleID >= 1 && tUser.RoleID <= 3) { tUser.RoleID = coachEditUserDto.RoleID; }
            if (tUser.UserName != coachEditUserDto.UserName && coachEditUserDto.UserName != "") { tUser.UserName = coachEditUserDto.UserName; }
            await _repo.CommitSave();
            return tUser;
        }
    }
}
