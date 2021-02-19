using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Model;
using Models;
using System;
using Xunit;

namespace Repository.Tests
{
    public class RepositoryTests
    {
        /// <summary>
        /// Tests the CommitSave() method of Repo
        /// </summary>
        [Fact]
        public async void TestForCommitSave()
        {
            var options = new DbContextOptionsBuilder<UserContext>().UseInMemoryDatabase(databaseName: "p2newsetuptest")
            .Options;



            using (var context = new UserContext(options))
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();

                Repo r = new Repo(context, new NullLogger<Repo>(),null,null);

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
                //add user to db
                r.Users.Add(user);
                await r.CommitSave();
            }

            using (var context = new UserContext(options))
            {
                //check that user is still in db with new context
                Repo r = new Repo(context, new NullLogger<Repo>(),null,null);

                Assert.NotEmpty(context.Users);
            }
        }

        /// <summary>
        /// Tests the GetUsers() method of Repo
        /// </summary>
        [Fact]
        public async void TestForGetUsers()
        {
            var options = new DbContextOptionsBuilder<UserContext>()
            .UseInMemoryDatabase(databaseName: "p2newsetuptest1")
            .Options;

            using (var context = new UserContext(options))
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();

                Repo r = new Repo(context, new NullLogger<Repo>(),null, null);

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
                //add user to db
                r.Users.Add(user);
                await r.CommitSave();
            }

            using (var context = new UserContext(options))
            {
                //check that user is still in db with new context
                Repo r = new Repo(context, new NullLogger<Repo>(),null, null);

                var listOfUsers = await r.GetUsers();
                Assert.NotNull(listOfUsers);
            }
        }

        /// <summary>
        /// Tests the GetUserById method of Repo
        /// </summary>
        [Fact]
        public async void TestForGetUserById()
        {
            var options = new DbContextOptionsBuilder<UserContext>()
            .UseInMemoryDatabase(databaseName: "p2newsetuptest2")
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

                Repo r = new Repo(context, new NullLogger<Repo>(), null, null);
                //add user to db
                r.Users.Add(user);
                await r.CommitSave();
            }

            using (var context = new UserContext(options))
            {
                //check that user is still in db with new context
                Repo r = new Repo(context, new NullLogger<Repo>(), null, null);

                var searchForUser = await r.GetUserById(user.Id);
                Assert.Equal(user.UserName, searchForUser.UserName);// True(searchForUser.Equals(user));
            }
        }
    }
}
