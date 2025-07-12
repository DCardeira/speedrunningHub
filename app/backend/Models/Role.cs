// Modelo que representa um papel (role) de utilizador na aplicação
namespace SpeedRunningHub.Models{
    public class Role{
        public int RoleId { get; set; }                  // Identificador único do papel
        public string Name { get; set; }                 // Nome do papel (ex: Runner, Moderator)
        public ICollection<UserRole> UserRoles { get; set; } // Relações com utilizadores
    }
}