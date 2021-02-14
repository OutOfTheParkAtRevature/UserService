using Models;
using Model.DataTransfer;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Xunit;

namespace Model.Tests
{
    public class ModelTests
    {
        /// <summary>
        /// Checks the data annotations of Models to make sure they aren't being violated
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private IList<ValidationResult> ValidateModel(object model)
        {
            var result = new List<ValidationResult>();
            var validationContext = new ValidationContext(model);
            Validator.TryValidateObject(model, validationContext, result, true);

            return result;
        }

        /// <summary>
        /// Validates the User Model works with proper data
        /// </summary>
        [Fact]
        public void ValidateUser()
        {
            var user = new ApplicationUser
            {
                Id = "13265262",
                UserName = "jerry",
                PasswordHash="jerryricehashed",
                FullName = "Jerry Rice",
                PhoneNumber = "111-111-1111",
                Email = "jerryrice@gmail.com",
                TeamID = Guid.NewGuid(),
                RoleName=Roles.HC,
                StatLineID = Guid.NewGuid()
            };

            var errorcount = ValidateModel(user).Count;
            Assert.Equal(0, errorcount);
        }



        /// <summary>
        /// Validates the authentication; dto works with proper data
        /// </summary>
        [Fact]
        public void ValidateUserForAuthenticationDto()
        {
            var authentication = new UserForAuthenticationDto
            {
                Email="something@email.com",
                Password="somethingSecure"
            };

            var errorcount = ValidateModel(authentication).Count;
            Assert.Equal(0, errorcount);
        }
        /// <summary>
        /// Makes sure the User Model doesn't accept empty fields for email and password
        /// </summary>
        [Fact]
        public void InvalidateUserForAuthenticationDto()
        {
            var authentication = new UserForAuthenticationDto
            {
            };

            var results = ValidateModel(authentication);
            Assert.True(results.Count > 0);
            Assert.Contains(results, v => v.MemberNames.Contains("Email"));
            Assert.Contains(results, v => v.MemberNames.Contains("Password"));
        }

    }
}
