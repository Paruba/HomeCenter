using Boiler.Server.Models;
using Microsoft.AspNetCore.SignalR;

namespace Boiler.Server.Hubs;

public class TemperatureHub : Hub
{
    public async Task SendTemperature(string user, TemperatureModel data)
    {
        await Clients.User(user).SendAsync("ReceiveTemperature", data);
    }
}
