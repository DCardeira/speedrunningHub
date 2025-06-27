using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SpeedRunningHub.Models;

namespace SpeedRunningHub.Data {
    public class DbInitializer {
        private readonly AppDbContext _context;
        public DbInitializer(AppDbContext context) {
            _context = context;
        }

        public async Task SeedAsync() {
            // Garante existência da BD
            await _context.Database.MigrateAsync();

            // Popula os Roles
            if (!await _context.Roles.AnyAsync()) {
                _context.Roles.AddRange(
                    new Role { Name = "Runner" },
                    new Role { Name = "Moderator" }
                );
                await _context.SaveChangesAsync();
            }

            // Popula o Default Moderator
            if (!await _context.Users.AnyAsync(u => u.UserRoles.Any(ur => ur.Role.Name == "Moderator"))) {
                var admin = new User {
                    UserId = Guid.NewGuid().ToString(),
                    UserName = "admin",
                    Email = "admin@fastruns.com"
                };

                var hasher = new PasswordHasher<User>();
                admin.PasswordHash = hasher.HashPassword(admin, "Admin#123");
                _context.Users.Add(admin);
                await _context.SaveChangesAsync();

                var moderatorRole = await _context.Roles.SingleAsync(r => r.Name == "Moderator");
                _context.UserRoles.Add(new UserRole {
                    UserId = admin.UserId,
                    RoleId = moderatorRole.RoleId
                });
                await _context.SaveChangesAsync();
            }

            // Popular jogos
            if (!await _context.Games.AnyAsync()) {
                _context.Games.AddRange(
                    new Game { Title = "Super Mario 64", Description = "3D platformer classic." },
                    new Game { Title = "Minecraft", Description = "Sandbox block-building game." }
                );
                await _context.SaveChangesAsync();
            }
        }
    }
}