using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Models;
using Moq;
using System;
using System.Collections.Generic;

namespace Service.Tests
{
    public class FakeUserManager : UserManager<ApplicationUser>
    {
        //FakeUserManager extends UserManager, but it uses an empty constructor so that we can mock it directly.
        public FakeUserManager()
            : base(new Mock<IUserStore<ApplicationUser>>().Object,
                  new Mock<IOptions<IdentityOptions>>().Object,
                  new Mock<IPasswordHasher<ApplicationUser>>().Object,
                  new IUserValidator<ApplicationUser>[0],
                  new IPasswordValidator<ApplicationUser>[0],
                  new Mock<ILookupNormalizer>().Object,
                  new Mock<IdentityErrorDescriber>().Object,
                  new Mock<IServiceProvider>().Object,
                  new Mock<ILogger<UserManager<ApplicationUser>>>().Object)
        { }

    }

    public class FakeRoleManager: RoleManager<IdentityRole>
    {
        //public FakeRoleManager()
        //    : base(new Mock<IRoleStore<IdentityRole>>().Object, 
        //          new Mock<IEnumerable<IRoleValidator<IdentityRole>>>().Object, 
        //          new Mock<ILookupNormalizer>().Object, 
        //          new Mock<IdentityErrorDescriber>().Object,
        //          new Mock<ILogger<RoleManager<IdentityRole>>>().Object)
        //{ }
        public FakeRoleManager()
            : base(new Mock<IRoleStore<IdentityRole>>().Object,
                  null,
                  null,
                  null,
                  null)
        { }
    }
}
