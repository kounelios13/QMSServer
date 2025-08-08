using MediatR;
using Microsoft.AspNetCore.SignalR;
using QMS.Hubs;

namespace QMS.Mediator
{
    public class TicketAssignedSignalRHandler : INotificationHandler<TicketAssignedEvent>
    {
        private readonly IHubContext<QueueHub> _hubContext;



        public TicketAssignedSignalRHandler(IHubContext<QueueHub> hubContext)
        {
            _hubContext = hubContext ?? throw new ArgumentNullException(nameof(hubContext));
        }

        async Task INotificationHandler<TicketAssignedEvent>.Handle(TicketAssignedEvent notification, CancellationToken cancellationToken)
        {
            await _hubContext.Clients.All.SendAsync("TicketAssigned",notification
                , cancellationToken);
        }
    }
}
