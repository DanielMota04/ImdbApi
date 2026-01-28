using ImdbApi.Models;
using Microsoft.EntityFrameworkCore;

namespace ImdbApi.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=localhost;Database=ImdbApiDb;Trusted_Connection=True;MultipleActiveResultSets=true");
        }
    }
}
