using Microsoft.AspNetCore.SignalR;

public class NotificationHub : Hub {
    // O servidor irá chamar métodos nos clientes.
    public async Task SendMessage(string user, string message) {
        await Clients.All.SendAsync("ReceiveMessage", user, message);
    }
}