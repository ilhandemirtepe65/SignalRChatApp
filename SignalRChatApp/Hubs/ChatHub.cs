using Microsoft.AspNetCore.SignalR;
using SignalRChatApp.Models;

namespace SignalRChatApp.Hubs
{
    public class ChatHub : Hub
    {
        public async Task SendMessage(UserMessage userMessage )
        {
            await Clients.All.SendAsync("ReceiveMessage", userMessage);
        }
    }
}
