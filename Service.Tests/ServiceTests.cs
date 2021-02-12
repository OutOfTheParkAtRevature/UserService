using Model;
using Models.DataTransfer;
using System;
using Xunit;

namespace Service.Tests
{
    public class ServiceTests
    {
        Logic _logic;
        public ServiceTests(Logic logic)
        {
            _logic = logic;

        }
        [Fact]
        public async void TestCreateUser()
        {
            CreateUserDto cud = new CreateUserDto
            {
                RoleName=Roles.PT,
                FullName="A Reasonable Name",
                Email="validemail@something.com",
                TeamID=Guid.NewGuid(),
                Password="Password123!",
                UserName="coolName53",
                PhoneNumber="555-3243"
            };

            var authResponse = await _logic.CreateUser(cud);

            Assert.True(authResponse.IsAuthSuccessful);

        }
    }
}
