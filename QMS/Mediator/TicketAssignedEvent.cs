using MediatR;

namespace QMS.Mediator;

public class TicketAssignedEvent : INotification
{
    public string TicketNumber { get; set; } = string.Empty;
    public string DeviceName { get; set; } = string.Empty;

    public DateTime AssignedAt { get; set; } = DateTime.UtcNow;

    public TicketAssignedEvent(string ticketNumber, string deviceName)
    {
        TicketNumber = ticketNumber;
        DeviceName = deviceName;
    }
}
