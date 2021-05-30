using Microsoft.EntityFrameworkCore;
using SpecflowEventualConsistency.Domain;

namespace SpecflowEventualConsistency.Infrastructure
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }
        
        public DbSet<Order> Orders { get; set; }
    }
}