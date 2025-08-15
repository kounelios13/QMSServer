using AutoMapper;
using Microsoft.EntityFrameworkCore;
using QMS.Db;
using QMS.DTO;

namespace QMS.DAL;

public class TicketRepository : ITicketRepository
{
    private readonly QmsDbContext _context;
    private readonly IMapper _mapper;
    public TicketRepository(QmsDbContext context , IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task AddTicket(Ticket ticket)
    {
        await _context.Tickets.AddAsync(ticket);
        await _context.SaveChangesAsync();
    }

    public async Task<Ticket?> GetTicketById(Guid id)
    {
        return await _context.Tickets.FindAsync(id);
    }

    public async Task<IEnumerable<TicketView>> GetAllTickets()
    {
        return await _context.Tickets
            .Include(s => s.FrontDeskTerminal)
            .Select(s => _mapper.Map<TicketView>(s))
            .ToListAsync();
    }
    

    public async Task<IEnumerable<Ticket>> GetTicketsByStatus(TicketStatus status)
    {
        return await _context.Tickets
            .Where(t => t.Status == status)
            .ToListAsync();
    }

    public async Task UpdateTicketStatus(Guid id, TicketStatus status)
    {
        var ticket = await _context.Tickets.FindAsync(id);
        if (ticket != null)
        {
            ticket.Status = status;
            _context.Tickets.Update(ticket);
            await _context.SaveChangesAsync();
        }
    }

    public async Task DeleteTicket(Guid id)
    {
        var ticket = await _context.Tickets.FindAsync(id);
        if (ticket != null)
        {
            _context.Tickets.Remove(ticket);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<Ticket?> GetNextAvailableTicket(string? frontDeskTerminalId = null)
    {
        var ticket = await _context.Tickets
            .Where(t => t.Status == TicketStatus.Pending)
            .OrderBy(t => t.IssuedAt)
            .FirstOrDefaultAsync();

        if (ticket == null)
        {
            return null; // No available tickets
        }
        ticket.Status = TicketStatus.InProgress;
        if (!string.IsNullOrEmpty(frontDeskTerminalId))
        {
            ticket.FrontDeskTerminalId = frontDeskTerminalId;
        }
        _context.Tickets.Update(ticket);
        await _context.SaveChangesAsync();
        return ticket;
    }

    public async Task ResetAll()
    {
        var tickets = await _context.Tickets.ToListAsync();
        foreach (var ticket in tickets)
        {
            ticket.Status = TicketStatus.Pending;
            ticket.FrontDeskTerminalId = null; // Unassign terminal
        }
        _context.Tickets.UpdateRange(tickets);
        await _context.SaveChangesAsync();
    }
}
