using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SpeedRunningHub.Models;
using Microsoft.AspNetCore.Identity;

// Classe responsável por inicializar e popular a base de dados
namespace SpeedRunningHub.Data {
    public class DbInitializer {
        private readonly AppDbContext _context;
        // Construtor: injeta o contexto da base de dados
        public DbInitializer(AppDbContext context) {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // Método para popular dados iniciais na base de dados
        public async Task SeedAsync() {
            // Aplica migrações se a base de dados for relacional
            if (_context.Database.IsRelational()) {
                await _context.Database.MigrateAsync();
            }

            // Popula os papéis (Roles) se não existirem
            if (!await _context.Roles.AnyAsync()) {
                _context.Roles.AddRange(
                    new Role { Name = "Runner" },
                    new Role { Name = "Moderator" }
                );
                await _context.SaveChangesAsync();
            }

            // Cria utilizador admin e atribui papel de Moderator se não existir
            if (!await _context.Users.AnyAsync(u => u.UserRoles.Any(ur => ur.Role.Name == "Moderator"))) {
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

            // Popula jogos iniciais se não existirem
            if (!await _context.Games.AnyAsync()) {
                _context.Games.AddRange(
                    new Game { Title = "Super Mario 64", Description = "Clássico 3D platformer." },
                    new Game { Title = "Minecraft", Description = "Jogo de SandBox de Blocos." }
                );
                await _context.SaveChangesAsync();
            }
        }
    }
}