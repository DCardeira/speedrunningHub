using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SpeedRunningHub.Models;

// Contexto da base de dados principal da aplicação
namespace SpeedRunningHub.Data {
    // Corrected the base class to include User, Role, and the key type (string)
    public class AppDbContext : IdentityDbContext<User, Role, string> {
        // Tabelas principais do modelo
        public DbSet<Game> Games { get; set; }
        public DbSet<Guide> Guides { get; set; }
        public DbSet<SpeedrunRecord> SpeedrunRecords { get; set; }
        public DbSet<GameImage> GameImages { get; set; }
        public DbSet<GuideImage> GuideImages { get; set; }

        // Construtor: injeta opções de configuração do contexto
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // Configuração das relações e chaves compostas
        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            base.OnModelCreating(modelBuilder);
        }
    }
}