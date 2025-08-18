using MediatR;

namespace QMS.Mediator;

public class TicketAssignedEvent : INotification
{
    public string TicketNumber { get; set; } = string.Empty;
    public string DeviceName { get; set; } = string.Empty;
    public string DeviceId { get; set; } = string.Empty;
    public DateTime AssignedAt { get; set; } = DateTime.UtcNow;

    public TicketAssignedEvent(string ticketNumber, string deviceName , string
        deviceId)
    {
        TicketNumber = ticketNumber;
        DeviceName = deviceName;
        DeviceId = deviceId;
    }
}
