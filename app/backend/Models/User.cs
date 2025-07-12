using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

// Modelo que representa um utilizador da aplicação
namespace SpeedRunningHub.Models{
    public class User : IdentityUser {
        public ICollection<SpeedrunRecord> SpeedrunRecords { get; set; } // Registos de speedrun submetidos
        public ICollection<Guide> Guides { get; set; }          // Guias criados pelo utilizador
    }
}