using Microsoft.EntityFrameworkCore;
using Models;

namespace Repo
{
    public class ProgContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        public ProgContext() { }

        public ProgContext(DbContextOptions<ProgContext> options) : base(options) { }


        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    modelBuilder.Entity<UserInbox>()
        //        .HasKey(c => new { c.UserID, c.MessageID });
        //    modelBuilder.Entity<RecipientList>()
        //        .HasKey(c => new { c.RecipientListID, c.RecipientID });
        //}
    }
}