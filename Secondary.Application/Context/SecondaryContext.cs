using Microsoft.EntityFrameworkCore;
using Secondary.Application.Domain;

namespace Secondary.Application.Context
{
    public class SecondaryContext(DbContextOptions options) : DbContext(options)
    {
        public DbSet<Order> Orders { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        { 
            base.OnModelCreating(modelBuilder);
        }
    }
}
