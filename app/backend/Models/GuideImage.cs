// Modelo que representa uma imagem associada a um guia
namespace SpeedRunningHub.Models{
    public class GuideImage{
        public int GuideImageId { get; set; }         // Identificador único da imagem
        public int GuideId { get; set; }              // Id do guia associado
        public string FileName { get; set; }          // Nome do ficheiro
        public string FilePath { get; set; }          // Caminho/URL do ficheiro
        public DateTime UploadedAt { get; set; }      // Data de upload
        public string UploadedByUserId { get; set; }  // Id do utilizador que fez upload

        public Guide Guide { get; set; }              // Navegação para o guia
        public User UploadedBy { get; set; }          // Navegação para o utilizador
    }
}