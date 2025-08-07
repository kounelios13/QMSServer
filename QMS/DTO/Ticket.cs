namespace QMS.DTO
{
    public class Ticket
    {
        public Guid Id { get; set; } = Guid.CreateVersion7();
        public DateTime IssuedAt { get; set; }
        public string? IPAddress { get; set; }
        public string TicketNumber { get; set; } = string.Empty;

        public TicketStatus Status { get; set; } = TicketStatus.Pending;
        // Foreign key (nullable, so unassigned is allowed)
        public string? FrontDeskTerminalId { get; set; }

        // Navigation property
        public FrontDeskTerminal? FrontDeskTerminal { get; set; }
    }

    public enum TicketStatus
    {
        Pending,
        InProgress,
        Completed,
        Cancelled
    }
}
