namespace QMS.DTO;

public class TicketView
{
    public Guid Id { get; set; } = Guid.CreateVersion7();
    public DateTime IssuedAt { get; set; }
    public string? IPAddress { get; set; }
    public string TicketNumber { get; set; } = string.Empty;

    public TicketStatus Status { get; set; } = TicketStatus.Pending;
    // Foreign key (nullable, so unassigned is allowed)
    // 
    public TerminalInfoView? FrontDeskTerminalInfo { get; set; } = null;
}
