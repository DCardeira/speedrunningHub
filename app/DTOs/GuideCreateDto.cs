using System.ComponentModel.DataAnnotations;

namespace SpeedRunningHub.DTOs {
    // DTO (Data Transfer Object) para a criação de um novo guia.
    public class GuideCreateDto {
        // O título do guia. É obrigatório e tem um limite de 100 caracteres.
        [Required]
        [StringLength(100)]
        public string Title { get; set; } = null!;

        // O conteúdo do guia. É um campo obrigatório.
        [Required]
        public string Content { get; set; } = null!;
    }
}