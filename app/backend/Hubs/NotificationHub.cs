using Microsoft.AspNetCore.SignalR;

public class NotificationHub : Hub {
    // Método que os clientes podem chamar (se necessário)
    // Mas principalmente, o servidor irá chamar métodos nos clientes.
    public async Task SendMessage(string user, string message) {
        await Clients.All.SendAsync("ReceiveMessage", user, message);
    }
}