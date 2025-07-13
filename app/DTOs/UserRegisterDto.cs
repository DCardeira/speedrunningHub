using System.ComponentModel.DataAnnotations;

namespace SpeedRunningHub.DTOs {
    // DTO para o registo de um novo utilizador.
    public class UserRegisterDto {
        [Required]
        public string UserName { get; set; } = null!;
        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;
        [Required]
        public string Password { get; set; } = null!;
    }

    // DTO para o login de um utilizador.
    public class UserLoginDto {
        [Required]
        public string UserName { get; set; } = null!;
        [Required]
        public string Password { get; set; } = null!;
    }

    // DTO para a resposta de autenticação, contendo o token e informação do utilizador.
    public class AuthResponseDto {
        public string Token { get; set; } = null!;
        public string UserName { get; set; } = null!;
        public IEnumerable<string> Roles { get; set; } = null!;
    }
}