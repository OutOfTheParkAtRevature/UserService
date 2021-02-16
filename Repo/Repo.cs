using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Model;
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
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserContext _userContext;
        private readonly ILogger _logger;
        public DbSet<ApplicationUser> Users;


        public Repo(UserContext userContext, ILogger<Repo> logger, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userContext = userContext;
            _logger = logger;
            this.Users = _userContext.ApplicationUsers;
            _roleManager = roleManager;
            _userManager = userManager;
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

        public async Task<ApplicationUser> GetUserByUsername(string username)
        {
            return await Users.FirstOrDefaultAsync(x => x.UserName == username);
        }

        public async Task SeedUsers()
        {
            if (!await _roleManager.RoleExistsAsync(Roles.A))
            {
                await _roleManager.CreateAsync(new IdentityRole(Roles.A));
                await _roleManager.CreateAsync(new IdentityRole(Roles.LM));
                await _roleManager.CreateAsync(new IdentityRole(Roles.HC));
                await _roleManager.CreateAsync(new IdentityRole(Roles.AC));
                await _roleManager.CreateAsync(new IdentityRole(Roles.PT));
                await _roleManager.CreateAsync(new IdentityRole(Roles.PL));
            }
            string[] userNames = { "CooperJoe", "RemerDoug", "ScolariKenny", "MartinRobert", "UnderwoodCarolyn", "CooperGavin", "RemerMax",
                "ScolariLiam", "MartinStewart", "UnderwoodColin", "WalshSteven", "AbrahamVictor", "AlsopDiana" };
            string[] passwords = { "Nonmeteorically1", "Airsickness2", "Exemplarily3", "Nonsobriety4", "Ileocolitis5", "Tittivating6", "Preconcentrating7",
                "Vascularization8", "Semidigested9", "Strumectomy0", "Fumigate1", "Geometrically2", "Superoccipital3" };
            string[] names = {  "Joe Cooper", "Doug Remer", "Kenny Scolari", "Robert Martin", "Carolyn Underwood", "Gavin Cooper", "Max Remer",
                "Liam Scolari", "Stewart Martin", "Colin Underwood", "Steven Walsh", "Victor Abraham", "Diana Alsop" };
            string[] phonenumbers = { "414-555-6548", "414-555-6453", "414-555-1056", "414-555-3546", "414-555-4356", "414-555-3685",
                "414-555-3257", "414-555-3428", "414-555-7839", "414-555-4523", "414-555-3658", "469-555-4387", "973-555-1654" };
            string[] emails = { "CooperJoe@Tigers.com", "RemerDoug@Tigers.com", "ScolariKenny@Tigers.com",
                "MartinRobert@Tigers.com", "UnderwoodCarolyn@Tigers.com", "CooperGavin@Tigers.com", "RemerMax@Tigers.com",
                "ScolariLiam@Tigers.com", "MartinStewart@Tigers.com", "UnderwoodColin@Tigers.com", "WalshSteven@Tigers.com",
                "AbrahamVictor@Tigers.com", "AlsopDiana@Tigers.com" };
            string[] roles = { "Parent", "Parent", "Parent", "Parent", "Parent", "Player", "Player", "Player", "Player", "Player", "Coach", "AssistantCoach" };
            Guid teamId = Guid.NewGuid();
            List<string> CarpoolList = new List<string>();
            for (int i = 0; i < userNames.Length; i++)
            {
                ApplicationUser user = new ApplicationUser
                {
                    FullName = names[i],
                    PhoneNumber = phonenumbers[i],
                    Email = emails[i],
                    SecurityStamp = Guid.NewGuid().ToString(),
                    UserName = userNames[i],
                    RoleName = roles[i],
                    TeamID = teamId
                };
                if (user.RoleName == "Parent")
                {
                    CarpoolList.Add(user.Id);
                }
                await Users.AddAsync(user);
            }
            await CommitSave();
        }
    }
}
