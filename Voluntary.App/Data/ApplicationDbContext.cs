using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Voluntary.App.Data.Entities;

namespace Voluntary.App.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Volunteer> Volunteers { get; set; }
        public DbSet<VoluntaryTask> Tasks { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<District> Districts { get; set; }
    }
}
