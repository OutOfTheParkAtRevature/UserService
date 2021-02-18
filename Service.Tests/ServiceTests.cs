using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Model;
using Models;
using Models.DataTransfer;
using Moq;
using Repository;
using System;
using System.Collections.Generic;
using Xunit;

namespace Service.Tests
{
    public class ServiceTests
    {

        IConfiguration _config;
        public ServiceTests()
        {
            //appsettings.test.json is copied from appsettings.json in main project
            _config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.test.json")
                .Build();
        }

        /// <summary>
        /// Checks that a proper LoggedInUserDto is generated from an ApplicationUser
        /// </summary>
        [Fact]
        public async void TestLoginUser()
        {
            //db requirement
            var options = new DbContextOptionsBuilder<UserContext>()
            .UseInMemoryDatabase(databaseName: "servicetestdb0")
            .Options;

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
            using (var context = new UserContext(options))
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();

                Repo repo = new Repo(context, new NullLogger<Repo>(), null, null);
                //add user to db
                repo.Users.Add(user);
                await repo.CommitSave();
            }
            using (var context = new UserContext(options))
            {
                Repo repo = new Repo(context, new NullLogger<Repo>(), null, null);

                var userManagerMock = new Mock<FakeUserManager>();
                userManagerMock.Setup(x => x.GetRolesAsync(user)).ReturnsAsync(() => new List<string>() { user.RoleName });


                Logic logic = new Logic(repo, userManagerMock.Object, new Mapper(), new JwtHandler(_config), new NullLogger<Repo>(), null);


                var result = await logic.LoginUser(user);

                Assert.Equal(user.Id, result.Id);
                Assert.Equal(user.FullName, result.FullName);
                Assert.Equal(user.UserName, result.UserName);
                Assert.Equal(user.PhoneNumber, result.PhoneNumber);
                Assert.Equal(user.Email, result.Email);
                Assert.Equal(user.TeamID, result.TeamID);
                Assert.Equal(user.RoleName, result.RoleName);
                Assert.NotEmpty(result.Token);
            }

        }
        /// <summary>
        /// Checks that a proper LoggedInUserDto is generated from an ApplicationUser
        /// </summary>
        [Fact]
        public async void TestCreateUser()
        {
            //db requirement
            var options = new DbContextOptionsBuilder<UserContext>()
            .UseInMemoryDatabase(databaseName: "servicetestdb1")
            .Options;

            var cud = new CreateUserDto
            {
                UserName = "jerry",
                Password = "Password123!",
                FullName = "Jerry Rice",
                PhoneNumber = "111-111-1111",
                Email = "jerryrice@gmail.com",
                TeamID = Guid.NewGuid(),
                RoleName = Roles.HC,
            };
            using (var context = new UserContext(options))
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();

                var userManagerMock = new Mock<FakeUserManager>();
                var roleManagerMock = new Mock<FakeRoleManager>();

                Repo repo = new Repo(context, new NullLogger<Repo>(), userManagerMock.Object, roleManagerMock.Object);

                userManagerMock.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success);

                //create callback to store in db because we don't have access to the usermanager to do it
                userManagerMock.Setup(x => x.AddToRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>())).Callback(async (ApplicationUser sentUser, string role) => { sentUser.RoleName = role; await repo.Users.AddAsync(sentUser);await repo.CommitSave();});

                
                roleManagerMock.Setup(x => x.RoleExistsAsync(Roles.A)).ReturnsAsync(true);


                //Repo repo = new Repo(context, new NullLogger<Repo>(),userManagerMock.Object,roleManagerMock.Object);

                Logic logic = new Logic(repo, userManagerMock.Object, new Mapper(), new JwtHandler(_config), new NullLogger<Repo>(), roleManagerMock.Object);

                var authResponse = await logic.CreateUser(cud);

                Assert.True(authResponse.IsAuthSuccessful);

                var result = await logic.GeUserByUsername(cud.UserName);


                Assert.Equal(cud.FullName, result.FullName);
                Assert.Equal(cud.UserName, result.UserName);
                Assert.Equal(cud.PhoneNumber, result.PhoneNumber);
                Assert.Equal(cud.Email, result.Email);
                Assert.Equal(cud.TeamID, result.TeamID);
                Assert.Equal(cud.RoleName, result.RoleName);
            }

        }
        [Fact]
        public async void TestGetUserById()
        {
            //db requirement
            var options = new DbContextOptionsBuilder<UserContext>()
            .UseInMemoryDatabase(databaseName: "servicetestdb2")
            .Options;


            var userManagerMock = new Mock<UserManager<ApplicationUser>>();
            var jwtHandlerMock = new Mock<JwtHandler>();
            var roleManagerMock = new Mock<RoleManager<IdentityRole>>();


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
            using (var context = new UserContext(options))
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();

                Repo repo = new Repo(context, new NullLogger<Repo>(),null,null);               
                //add user to db
                repo.Users.Add(user);
                await repo.CommitSave();
            }
            using (var context = new UserContext(options))
            {
                //check that user is still in db with new context
                Repo repo = new Repo(context, new NullLogger<Repo>(), null,null);
                //mock logic requirements for finduserbyId
                Logic logic = new Logic(repo, null, null, null, new NullLogger<Repo>(), null);

                var result = await logic.GetUserById(user.Id);

                Assert.Equal(user.UserName,result.UserName);
            }
        }
        [Fact]
        public async void TestGetUserByUsername()
        {
            //db requirement
            var options = new DbContextOptionsBuilder<UserContext>()
            .UseInMemoryDatabase(databaseName: "servicetestdb3")
            .Options;

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
            using (var context = new UserContext(options))
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();

                Repo repo = new Repo(context, new NullLogger<Repo>(),null,null);
                //add user to db
                repo.Users.Add(user);
                await repo.CommitSave();
            }
            using (var context = new UserContext(options))
            {
                //check that user is still in db with new context
                Repo repo = new Repo(context, new NullLogger<Repo>(),null,null);
                Logic logic = new Logic(repo, null, null, null, new NullLogger<Repo>(), null);

                var result = await logic.GeUserByUsername(user.UserName);

                Assert.Equal(user.Id, result.Id);
            }
        }
    }
    
}
