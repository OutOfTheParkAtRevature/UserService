using Models;
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
            var user = new User
            {
                Id = "13265262",
                UserName = "jerry",
                Password = "jerryrice",
                PasswordHash="jerryricehashed",
                FullName = "Jerry Rice",
                PhoneNumber = "111-111-1111",
                Email = "jerryrice@gmail.com",
                TeamID = 1,
                RoleID = 1,
                StatLineID = Guid.NewGuid()
            };

            var errorcount = ValidateModel(user).Count;
            Assert.Equal(0, errorcount);
        }

        /// <summary>
        /// Makes sure the User Model doesn't accept invalid data
        /// </summary>
        [Fact]
        public void InvalidateUser()
        {
            //invalid email
            var user = new User
            {
                Id = "13265262",
                UserName = "jerry",
                Password = "jerryrice",
                PasswordHash = "jerryricehashed",
                FullName = "Jerry Rice",
                PhoneNumber = "111-111-1111",
                Email = "1234",
                TeamID = 1,
                RoleID = 1,
                StatLineID = Guid.NewGuid()
            };


            var results = ValidateModel(user);
            Assert.True(results.Count > 0);
            Assert.Contains(results, v => v.MemberNames.Contains("Email"));
        }

        /// <summary>
        /// Makes sure Role Model works with valid data
        /// </summary>
        [Fact]
        public void ValidateRole()
        {
            var role = new Role()
            {
                
                Id = "1543523543",
                Name = "Coach",
            };

            var results = ValidateModel(role);
            Assert.True(results.Count == 0);
        }
    }
}
