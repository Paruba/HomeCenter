using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace Boiler.Server.Hubs;

public class HubUserIdProvider : IUserIdProvider
{
    public string GetUserId(HubConnectionContext connection)
    {
        var nameIdentifier =  connection.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return nameIdentifier;
    }
}
