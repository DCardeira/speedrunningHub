using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SpeedRunningHub.Models;

namespace SpeedRunningHub.Data {
    public class AppDbContext : IdentityDbContext<User, Role, string> {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        public DbSet<Game> Games { get; set; }
        public DbSet<SpeedrunRecord> SpeedrunRecords { get; set; }
        public DbSet<Guide> Guides { get; set; }
        public DbSet<GameImage> GameImages { get; set; }
        public DbSet<GuideImage> GuideImages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder){
            base.OnModelCreating(modelBuilder);

            // configuração das relações...
        }
    }
}