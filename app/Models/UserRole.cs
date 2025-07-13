// Modelo que representa a relação entre utilizador e papel (role)
namespace SpeedRunningHub.Models{
    public class UserRole{
        public string UserId { get; set; }   // Id do utilizador
        public int RoleId { get; set; }      // Id do papel
        public User User { get; set; }       // Navegação para o utilizador
        public Role Role { get; set; }       // Navegação para o papel
    }
}