
using Microsoft.EntityFrameworkCore;
using SpeedRunningHub.Models;

namespace SpeedRunningHub.Data{
    public class AppDbContext : DbContext{
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<Game> Games { get; set; }
        public DbSet<SpeedrunRecord> SpeedrunRecords { get; set; }
        public DbSet<Guide> Guides { get; set; }
        public DbSet<GameImage> GameImages { get; set; }
        public DbSet<GuideImage> GuideImages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder){
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<UserRole>()
                .HasKey(ur => new { ur.UserId, ur.RoleId });

            // configuração das relações...
        }
    }
}