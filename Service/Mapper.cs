using Models;
using Models.DataTransfer;

namespace Service
{
    public class Mapper
    {
        public UserDto ConvertUserToUserDto(ApplicationUser user)
        {
            UserDto convertedUser = new UserDto()
            {
                Id = user.Id,
                FullName = user.FullName,
                UserName = user.UserName,
                PhoneNumber = user.PhoneNumber,
                Email = user.Email,
                TeamID = user.TeamID,
                RoleName = user.RoleName
            };
            return convertedUser;
        }
        public UserLoggedInDto ConvertUserToUserLoggedInDto(ApplicationUser user)
        {
            UserLoggedInDto convertedUser = new UserLoggedInDto()
            {
                Id = user.Id,
                FullName = user.FullName,
                UserName = user.UserName,
                PhoneNumber = user.PhoneNumber,
                Email = user.Email,
                TeamID = user.TeamID,
                RoleName = user.RoleName
            };
            return convertedUser;
        }
    }
}
