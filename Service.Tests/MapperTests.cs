using Model;
using Models;
using System;

using Xunit;


namespace Service.Tests
{
    public class MapperTests
    {
        /// <summary>
        /// Tests converting an ApplicationUser to a UserDto
        /// </summary>
        [Fact]
        public void TestConvertUserToUserDto()
        {
            var mapper = new Mapper();
            var user = new ApplicationUser
            {
                Id = "13265262",
                UserName = "jerry",
                PasswordHash = "jerryricehashed",
                FullName = "Jerry Rice",
                PhoneNumber = "111-111-1111",
                Email = "jerryrice@gmail.com",
                TeamID = Guid.NewGuid(),
                RoleName = Roles.HC,
                StatLineID = Guid.NewGuid()
            };

            var userDto = mapper.ConvertUserToUserDto(user);

            Assert.Equal(user.Id, userDto.Id);
            Assert.Equal(user.FullName, userDto.FullName); 
            Assert.Equal(user.UserName, userDto.UserName);
            Assert.Equal(user.PhoneNumber, userDto.PhoneNumber);
            Assert.Equal(user.Email, userDto.Email);
            Assert.Equal(user.TeamID, userDto.TeamID);
            Assert.Equal(user.RoleName, userDto.RoleName);
        }

        /// <summary>
        /// Tests converting an ApplicationUser to a LoggedInUserDto including an empty token
        /// </summary>
        [Fact]
        public void TestConvertUserToLoggedInUserDto()
        {
            var mapper = new Mapper();
            var user = new ApplicationUser
            {
                Id = "13265262",
                UserName = "jerry",
                PasswordHash = "jerryricehashed",
                FullName = "Jerry Rice",
                PhoneNumber = "111-111-1111",
                Email = "jerryrice@gmail.com",
                TeamID = Guid.NewGuid(),
                RoleName = Roles.HC,
                StatLineID = Guid.NewGuid()
            };

            var loggedInUserDto = mapper.ConvertUserToUserLoggedInDto(user);

            Assert.Equal(user.Id, loggedInUserDto.Id);
            Assert.Equal(user.FullName, loggedInUserDto.FullName);
            Assert.Equal(user.UserName, loggedInUserDto.UserName);
            Assert.Equal(user.PhoneNumber, loggedInUserDto.PhoneNumber);
            Assert.Equal(user.Email, loggedInUserDto.Email);
            Assert.Equal(user.TeamID, loggedInUserDto.TeamID);
            Assert.Equal(user.RoleName, loggedInUserDto.RoleName);
            Assert.Null(loggedInUserDto.Token);
        }
    }
}
