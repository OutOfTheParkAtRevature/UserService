using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class Repo
    {
        private readonly UserContext _userContext;
        private readonly ILogger _logger;
        public DbSet<ApplicationUser> Users;


        public Repo(UserContext userContext, ILogger<Repo> logger)
        {
            _userContext = userContext;
            _logger = logger;
            this.Users = _userContext.ApplicationUsers;
        }

        // Access SaveChanges from Logic class
        public async Task CommitSave()
        {
            await _userContext.SaveChangesAsync();
        }
        // Context accessors
        public async Task<IEnumerable<ApplicationUser>> GetUsers()
        {
            List<ApplicationUser> uList = await Users.ToListAsync();
            return uList;
        }
        public async Task<ApplicationUser> GetUserById(string id)
        {
            return await Users.FindAsync(id);
        }
    }
}
