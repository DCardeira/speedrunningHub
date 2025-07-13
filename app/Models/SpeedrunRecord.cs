using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SpeedRunningHub.Models {
    // Modelo que representa um registo de speedrun submetido por um utilizador
    public class SpeedrunRecord {
        [Key]
        public int RunId { get; set; }                  // Identificador único do registo

        public string RecordId { get; set; } = Guid.NewGuid().ToString(); // Identificador global único

        [ForeignKey(nameof(Game))]
        public int GameId { get; set; }                 // Id do jogo associado

        [ForeignKey(nameof(User))]
        public string UserId { get; set; } = null!;     // Id do utilizador que submeteu

        public TimeSpan Time { get; set; }              // Tempo da speedrun
        public bool IsApproved { get; set; }            // Indica se o registo foi aprovado
        public DateTime DateSubmitted { get; set; }     // Data de submissão
        public string VideoLink { get; set; } = null!;  // Link do vídeo da speedrun

        public Game Game { get; set; } = null!;         // Navegação para o jogo
        public User User { get; set; } = null!;         // Navegação para o utilizador
    }
}