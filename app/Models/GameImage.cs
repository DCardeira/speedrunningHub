using System.ComponentModel.DataAnnotations;

// Modelo que representa uma imagem associada a um jogo
namespace SpeedRunningHub.Models {
    public class GameImage {
        [Key]
        public int ImageId { get; set; }             // Identificador único da imagem
        public int GameId { get; set; }              // Id do jogo associado
        public string ImagePath { get; set; }        // Caminho/URL do ficheiro
        public DateTime UploadedAt { get; set; }     // Data de upload
        public string UploadedByUserId { get; set; } // Id do utilizador que fez upload

        public Game Game { get; set; }               // Navegação para o jogo
        public User UploadedBy { get; set; }         // Navegação para o utilizador
    }
}