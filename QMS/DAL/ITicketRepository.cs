using QMS.DTO;

namespace QMS.DAL
{
    public interface ITicketRepository
    {

        Task AddTicket(Ticket ticket);
        Task<Ticket?> GetTicketById(Guid id);
        Task<IEnumerable<TicketView>> GetAllTickets();
        Task<IEnumerable<Ticket>> GetTicketsByStatus(TicketStatus status);
        Task UpdateTicketStatus(Guid id, TicketStatus status);
        Task DeleteTicket(Guid id);

        Task<Ticket> GetNextAvailableTicket(string? frontDeskTerminalId = null);

        Task ResetAll();

    }
}
