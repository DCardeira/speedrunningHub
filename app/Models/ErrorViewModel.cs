// Modelo utilizado para exibir informações de erro na interface
namespace SpeedRunningHub.Models;

public class ErrorViewModel {
    public string? RequestId { get; set; } // Identificador da requisição (útil para rastreamento)
    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId); // Indica se o RequestId deve ser exibido
}