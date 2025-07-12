using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SpeedRunningHub.Models;
using Microsoft.AspNetCore.Identity;

namespace SpeedRunningHub.Data {
    public class DbInitializer {
        private readonly AppDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;
        public DbInitializer(AppDbContext context, UserManager<User> userManager, RoleManager<Role> roleManager) {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

         public async Task SeedAsync() {
            if (_context.Database.IsRelational()) {
                await _context.Database.MigrateAsync();
            }

            // Seed Roles
            if (!await _roleManager.RoleExistsAsync("Runner")) {
                await _roleManager.CreateAsync(new Role { Name = "Runner" });
            }
            if (!await _roleManager.RoleExistsAsync("Moderator")) {
                await _roleManager.CreateAsync(new Role { Name = "Moderator" });
            }

            // Seed Default Moderator
            var moderatorUsers = await _userManager.GetUsersInRoleAsync("Moderator");
            if (!moderatorUsers.Any()) {
                var admin = new User {
                    UserName = "admin",
                    Email = "admin@fastruns.com",
                    EmailConfirmed = true // Important for some Identity features
                };

                // Create the user with a password
                var result = await _userManager.CreateAsync(admin, "Admin#123");
                if (result.Succeeded) {
                    // Assign the 'Moderator' role
                    await _userManager.AddToRoleAsync(admin, "Moderator");
                }
            }

            // Seed Games
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