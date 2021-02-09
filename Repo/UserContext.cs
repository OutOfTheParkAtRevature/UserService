﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Models;

namespace Repository
{
    public class UserContext : IdentityDbContext<User>
    {
        //public DbSet<User> Users { get; set; }

        public UserContext() { }

        public UserContext(DbContextOptions<UserContext> options) : base(options) { }
    }
}