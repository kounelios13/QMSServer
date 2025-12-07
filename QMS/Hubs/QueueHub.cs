using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Authorization;
using QMS.Configuration;

namespace QMS.Hubs;

[Authorize(Policy = AuthorizationPolicies.PublicPolicy)] // Require authentication for SignalR connections
public class QueueHub : Hub
{
    private readonly ILogger<QueueHub> _logger;
    public QueueHub(ILogger<QueueHub> logger)
    {
        _logger = logger;
    }

    public override Task OnConnectedAsync()
    {
        _logger.LogInformation("Acquired a new connection");
        return base.OnConnectedAsync();
    }
}
