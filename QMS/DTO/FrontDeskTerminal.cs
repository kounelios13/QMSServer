using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;

namespace QMS.DTO;

public class FrontDeskTerminal
{

    [MaxLength(191)]
    public string DeviceId { get; set; } = string.Empty;
    public DateTime LastSeen { get; set; } = DateTime.UtcNow;
    [MaxLength(15)]
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
