using Microsoft.EntityFrameworkCore;
using CampRating.Models;

namespace CampRating.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<CampingSite> CampingSites { get; set; }
        public DbSet<Review> Reviews { get; set; }
    }
}
