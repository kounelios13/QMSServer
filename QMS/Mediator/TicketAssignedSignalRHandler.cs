using MediatR;
using Microsoft.AspNetCore.SignalR;
using QMS.DAL;
using QMS.Hubs;

namespace QMS.Mediator
{
    public class TicketAssignedSignalRHandler : INotificationHandler<TicketAssignedEvent>
    {
        private readonly IHubContext<QueueHub> _hubContext;
        private readonly IFrontDeskRepository _deviceRepo;


        public TicketAssignedSignalRHandler(IHubContext<QueueHub> hubContext , IFrontDeskRepository deviceRepo)
        {
            _hubContext = hubContext ?? throw new ArgumentNullException(nameof(hubContext));
            _deviceRepo = deviceRepo ?? throw new ArgumentNullException(nameof(deviceRepo));
        }

        async Task INotificationHandler<TicketAssignedEvent>.Handle(TicketAssignedEvent notification, CancellationToken cancellationToken)
        {
            await _hubContext.Clients.All.SendAsync("TicketAssigned",notification
                , cancellationToken);

            var device =  _deviceRepo.GetDeviceById(notification.DeviceId); 
            _deviceRepo.UpdateDeviceLastSeen(notification.DeviceId, DateTime.UtcNow);
        }
    }
}
