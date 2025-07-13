// Modelo que representa um guia associado a um jogo
namespace SpeedRunningHub.Models{
    public class Guide{
        public int GuideId { get; set; }           // Identificador único do guia
        public int GameId { get; set; }            // Id do jogo associado
        public string UserId { get; set; }         // Id do utilizador que criou o guia
        public string Title { get; set; }          // Título do guia
        public string Content { get; set; }        // Conteúdo do guia
        public bool IsApproved { get; set; }       // Indica se o guia foi aprovado
        public DateTime DateCreated { get; set; }  // Data de criação do guia

        public Game Game { get; set; }             // Navegação para o jogo
        public User User { get; set; }             // Navegação para o utilizador
        public ICollection<GuideImage> GuideImages { get; set; } // Imagens associadas ao guia
    }
}