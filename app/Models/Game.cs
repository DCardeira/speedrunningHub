// Modelo que representa um jogo na aplicação
namespace SpeedRunningHub.Models{
    public class Game{
        public int GameId { get; set; } // Identificador único do jogo
        public string Title { get; set; } // Título do jogo
        public string Description { get; set; } // Descrição do jogo
        public DateTime ReleaseDate { get; set; } // Data de lançamento do jogo
        public ICollection<SpeedrunRecord> SpeedrunRecords { get; set; } // Registos de speedrun associados
        public ICollection<Guide> Guides { get; set; } // Guias associados ao jogo
        public ICollection<GameImage> GameImages { get; set; } // Imagens associadas ao jogo
    }
}