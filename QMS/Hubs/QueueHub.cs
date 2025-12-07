using Microsoft.AspNetCore.SignalR;

namespace QMS.Hubs;

// No authorization required - allows public kiosks to connect and receive real-time updates
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
