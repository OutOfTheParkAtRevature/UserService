using System;

namespace Logic
{
    public class Logic
    {
        /// <summary>
        /// UserID -> Repo.GetUser
        /// </summary>
        /// <param name="id">user id</param>
        /// <returns>user</returns>
        public async Task<User> GetUserById(Guid id)
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
        /// Takes user input, creates authentication data
        /// </summary>
        /// <param name="createUser">User info sent from controller</param>
        /// <returns>UserLoggedInDto</returns>
        public async Task<UserLoggedInDto> RegisterUser(CreateUserDto createUser)
        {
            using var hmac = new HMACSHA512();
            User user = new User()
            {
                UserName = createUser.UserName,
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(createUser.Password)),
                PasswordSalt = hmac.Key,
                FullName = createUser.FullName,
                PhoneNumber = createUser.PhoneNumber,
                Email = createUser.Email,
                TeamID = createUser.TeamID,
                RoleID = createUser.RoleID
            };
            if (user.RoleID == 3)
            {
                var team = await _repo.teams.FirstOrDefaultAsync(x => x.TeamID == user.TeamID);
                await _repo.recipientLists.AddAsync(new RecipientList() { RecipientListID = team.CarpoolID, RecipientID = user.UserID });
            }
            await _repo.users.AddAsync(user);
            await _repo.CommitSave();
            _logger.LogInformation("User created");
            UserLoggedInDto newUser = _mapper.ConvertUserToUserLoggedInDto(user);
            newUser.Token = _token.CreateToken(user);
            return newUser;
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
            bool userExists = await _repo.users.AnyAsync(x => x.UserName == username && x.Email == email);
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
        public async Task<User> LoginUser(LoginDto loginDto)
        {
            return await _repo.users.SingleOrDefaultAsync(x => x.UserName == loginDto.UserName);
        }
        /// <summary>
        /// Verifies password passed from user input
        /// </summary>
        /// <param name="user"></param>
        /// <param name="loginDto"></param>
        /// <returns>UserLoggedInDto</returns>
        public async Task<UserLoggedInDto> CheckPassword(Task<User> user, LoginDto loginDto)
        {
            using var hmac = new HMACSHA512(user.Result.PasswordSalt);
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));
            for (int i = 0; i < computedHash.Length; i++)
            {
                if (computedHash[i] != user.Result.PasswordHash[i])
                {
                    return null;
                }
            }
            User loggedIn = await user;
            UserLoggedInDto loggedInUser = _mapper.ConvertUserToUserLoggedInDto(loggedIn);
            loggedInUser.Token = _token.CreateToken(loggedIn);
            return loggedInUser;
        }
        /// <summary>
        /// Delete user from context by ID
        /// </summary>
        /// <param name="id">UserID</param>
        /// <returns>deleted User</returns>
        public async Task<User> DeleteUser(Guid id)
        {
            User user = await GetUserById(id);
            if (user != null)
            {
                _repo.users.Remove(user);
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
        public async Task<User> AddUserRole(Guid userId, int roleId)
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
        public async Task<User> EditUser(Guid userId, EditUserDto editUserDto)
        {
            User tUser = await GetUserById(userId);
            if (tUser.FullName != editUserDto.FullName && editUserDto.FullName != "") { tUser.FullName = editUserDto.FullName; }
            if (tUser.Email != editUserDto.Email && editUserDto.Email != "") { tUser.Email = editUserDto.Email; }
            if (tUser.Password != editUserDto.Password && editUserDto.Password != "") { tUser.Password = editUserDto.Password; }
            if (tUser.PhoneNumber != editUserDto.PhoneNumber && editUserDto.PhoneNumber != "") { tUser.PhoneNumber = editUserDto.PhoneNumber; }
            await _repo.CommitSave();
            return tUser;
        }
        /// <summary>
        /// Same as above, more options for higher user level
        /// </summary>
        /// <param name="userId">User to edit</param>
        /// <param name="coachEditUserDto">New information</param>
        /// <returns>modified User</returns>
        public async Task<User> CoachEditUser(Guid userId, CoachEditUserDto coachEditUserDto)
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
        // Teams
        /// <summary>
        /// Get Team by ID
        /// </summary>
        /// <param name="id">TeamID</param>
        /// <returns>Team</returns>
        public async Task<Team> GetTeamById(int id)
        {
            return await _repo.GetTeamById(id);
        }
        /// <summary>
        /// Get list of Teams
        /// </summary>
        /// <returns>Teams</returns>
        public async Task<IEnumerable<Team>> GetTeams()
        {
            return await _repo.GetTeams();
        }
        /// <summary>
        /// Edit Team
        /// </summary>
        /// <param name="id">Team to edit</param>
        /// <param name="editTeamDto">New information</param>
        /// <returns>modified Team</returns>
        public async Task<Team> EditTeam(int id, EditTeamDto editTeamDto)
        {
            Team tTeam = await GetTeamById(id);
            if (tTeam.Name != editTeamDto.Name && editTeamDto.Name != "") { tTeam.Name = editTeamDto.Name; }
            if (tTeam.Wins != editTeamDto.Wins && editTeamDto.Wins >= 0) { tTeam.Wins = editTeamDto.Wins; }
            if (tTeam.Losses != editTeamDto.Losses && editTeamDto.Losses >= 0) { tTeam.Losses = editTeamDto.Losses; }
            await _repo.CommitSave();
            return tTeam;
        }
    }
}
