using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SpeedRunningHub.Models;

// Contexto da base de dados principal da aplicação
namespace SpeedRunningHub.Data{
    public class AppDbContext : IdentityDbContext<User, Role, string> {
        // Construtor: injeta opções de configuração do contexto
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        // Tabelas principais do modelo
        public DbSet<Game> Games { get; set; }           // Jogos
        public DbSet<SpeedrunRecord> SpeedrunRecords { get; set; } // Registos de speedrun
        public DbSet<Guide> Guides { get; set; }         // Guias
        public DbSet<GameImage> GameImages { get; set; } // Imagens de jogos
        public DbSet<GuideImage> GuideImages { get; set; } // Imagens de guias

        // Configuração das relações e chaves compostas
        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            base.OnModelCreating(modelBuilder);
        }
    }
}