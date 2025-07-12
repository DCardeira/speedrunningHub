using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SpeedRunningHub.Models;
using Microsoft.AspNetCore.Identity;

// Classe responsável por inicializar e popular a base de dados
namespace SpeedRunningHub.Data {
    public class DbInitializer {
        private readonly AppDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;
        // Construtor: injeta o contexto da base de dados
        public DbInitializer(AppDbContext context, UserManager<User> userManager, RoleManager<Role> roleManager) {
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
            if (!await _roleManager.RoleExistsAsync("Runner")) {
                await _roleManager.CreateAsync(new Role { Name = "Runner" });
            }
            if (!await _roleManager.RoleExistsAsync("Moderator")) {
                await _roleManager.CreateAsync(new Role { Name = "Moderator" });
            }

            // Cria utilizador admin e atribui papel de Moderator se não existir
            if (!(await _userManager.GetUsersInRoleAsync("Moderator")).Any()) {
                var admin = new User {
                    UserName = "admin",
                    Email = "admin@fastruns.com",
                    EmailConfirmed = true
                };
                var result = await _userManager.CreateAsync(admin, "Admin#123");
                if (result.Succeeded) {
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