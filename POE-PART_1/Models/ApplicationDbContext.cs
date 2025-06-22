using Microsoft.EntityFrameworkCore;
using POE_PART_1.Models;

namespace POE_PART_1.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options): base(options) 
        { } 
        public DbSet<Venuee> Venuee { get; set; }
        public DbSet<Evvent> Evvent { get; set; }

        public DbSet<Bookingg> Bookingg { get; set; }

        public DbSet<EvventType> EvventType { get; set; }

    }
}
