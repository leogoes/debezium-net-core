using Microsoft.EntityFrameworkCore;
using Primary.Application.Domain;

namespace Primary.Application.Context
{
    public class PrimaryContext(DbContextOptions options) : DbContext(options)
    {
        public DbSet<Person> Persons { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        { 
            base.OnModelCreating(modelBuilder);
        }
    }
}
