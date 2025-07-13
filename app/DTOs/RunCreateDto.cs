using System.ComponentModel.DataAnnotations;

namespace SpeedRunningHub.DTOs {
    // DTO para criar um novo registo de speedrun.
    public class RunCreateDto {
        // O tempo que o jogador demorou. É um campo obrigatório.
        [Required]
        public TimeSpan Time { get; set; }

        // O link para o vídeo de prova. É um campo obrigatório e deve ser um URL válido.
        [Required]
        [Url]
        public string VideoLink { get; set; } = null!;
    }
}