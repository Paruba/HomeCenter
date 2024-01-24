using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace Boiler.Server.Hubs;

public class StreamNotificationHub : Hub
{
    public async Task SendMessage(string user, string message)
    {
        await Clients.User(user).SendAsync("ReceiveMessage", message);
    }

    public async Task SendImage(string user, string base64Image)
    {
        await Clients.User(user).SendAsync("ReceiveImage", base64Image);
    }

    public async Task SendVideo(string user, string videoUrl)
    {
        await Clients.User(user).SendAsync("ReceiveVideo", videoUrl);
    }

    public override async Task OnConnectedAsync()
    {
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception exception)
    {
        await base.OnDisconnectedAsync(exception);
    }
}
