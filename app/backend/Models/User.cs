// Modelo que representa um utilizador da aplicação
namespace SpeedRunningHub.Models{
    public class User{
        public string UserId { get; set; }                      // Identificador único do utilizador
        public string UserName { get; set; }                    // Nome de utilizador
        public string Email { get; set; }                       // Email do utilizador
        public string PasswordHash { get; set; }                // Hash da password
        public ICollection<UserRole> UserRoles { get; set; }    // Relações com papéis (roles)
        public ICollection<SpeedrunRecord> SpeedrunRecords { get; set; } // Registos de speedrun submetidos
        public ICollection<Guide> Guides { get; set; }          // Guias criados pelo utilizador
    }
}