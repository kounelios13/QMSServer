using Microsoft.EntityFrameworkCore;
using System;

namespace QMS.DTO;

public class FrontDeskTerminal
{
    public string DeviceId { get; set; } = string.Empty;
    public DateTime LastSeen { get; set; } = DateTime.UtcNow;
    public string IPAddress { get; set; } = string.Empty;
    public string DeviceName { get; set; } = string.Empty;
    public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
    public FrontDeskTerminal(string deviceId)
    {
        DeviceId = deviceId;
    }
    public FrontDeskTerminal() { }
  
    public override string ToString()
    {
        return $"DeviceId: {DeviceId}, LastSeen: {LastSeen}";
    }
}
