using Microsoft.Extensions.Configuration;
using Model;
using Models;
using System;
using Xunit;

namespace Service.Tests
{
    public class JwtHandlerTests
    {
        IConfiguration _config;
        public JwtHandlerTests()
        {
            //appsettings.test.json is copied from appsettings.json in main project
            _config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.test.json")
                .Build();
        }

        /// <summary>
        /// Ensures that a valid SigningCredentials is returned
        /// </summary>
        [Fact]
        public void TestGetSigningCredentials()
        {
            var jwtHandler = new JwtHandler(_config);
            var credentials = jwtHandler.GetSigningCredentials();
            Assert.NotNull(credentials);
        }
        /// <summary>
        /// Ensures that all claims created for a user have a type and a value.
        /// </summary>
        [Fact]
        public void TestGetClaims()
        {
            var jwtHandler = new JwtHandler(_config);
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
            var claims = jwtHandler.GetClaims(user);
            foreach(var claim in claims)
            {
                Assert.NotEmpty(claim.Type);
                Assert.NotEmpty(claim.Value);
            }
        }
        [Fact]
        public void TestGenerateTokenOptions()
        {
            var jwtHandler = new JwtHandler(_config);

            var credentials = jwtHandler.GetSigningCredentials();
            Assert.NotNull(credentials);

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
            var claims = jwtHandler.GetClaims(user);

            var token = jwtHandler.GenerateTokenOptions(credentials, claims);

            Assert.NotEmpty(token.Issuer);
            Assert.NotEmpty(token.Audiences);
            Assert.NotEmpty(token.Claims);
            Assert.NotNull(token.SigningCredentials);


        }


    }
}
