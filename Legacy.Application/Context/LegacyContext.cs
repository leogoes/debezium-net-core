using Legacy.Application.Domain;
using Microsoft.EntityFrameworkCore;

namespace Legacy.Application.Context
{
    public class LegacyContext(DbContextOptions options) : DbContext(options)
    {
        public DbSet<Order> Orders { get; set; }
        public DbSet<Person> Persons { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        { 
            base.OnModelCreating(modelBuilder);
        }
    }
}
