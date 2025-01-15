using Microsoft.EntityFrameworkCore;
using PhoneBookAPI.Models.Contacts;

namespace PhoneBookAPI.Models
{
    public class PhoneBookDbContext : DbContext
    {
        public PhoneBookDbContext(DbContextOptions<PhoneBookDbContext> options)
            : base(options)
        {
        }

        public DbSet<Contact> Contacts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Contact>()
                .HasIndex(c => c.PhoneNumber)
                .IsUnique();

        }
    }
}