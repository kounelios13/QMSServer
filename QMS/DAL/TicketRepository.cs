using AutoMapper;
using Microsoft.EntityFrameworkCore;
using QMS.Db;
using QMS.DTO;

namespace QMS.DAL;

public class TicketRepository : ITicketRepository
{
    private readonly QmsDbContext _context;
    private readonly IMapper _mapper;
    private static readonly SemaphoreSlim _counterLock = new SemaphoreSlim(1, 1);
    
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

    public async Task<Ticket> GetNextAvailableTicket(string? frontDeskTerminalId = null)
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

    public async Task<string> GenerateNextTicketNumber()
    {
        // Use a semaphore to ensure only one thread can generate a ticket number at a time
        // Wait for up to 30 seconds to prevent indefinite blocking
        if (!await _counterLock.WaitAsync(TimeSpan.FromSeconds(30)))
        {
            throw new TimeoutException("Timeout waiting to generate ticket number");
        }
        
        try
        {
            // Use a transaction to ensure atomicity
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Get or create the ticket counter
                var counter = await _context.TicketCounters.FirstOrDefaultAsync(c => c.Id == 1);
                
                if (counter == null)
                {
                    // Counter doesn't exist, create it
                    counter = new TicketCounter
                    {
                        Id = 1,
                        CurrentNumber = 1, // Start at 1
                        LastUpdated = DateTime.UtcNow
                    };
                    
                    try
                    {
                        await _context.TicketCounters.AddAsync(counter);
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateException)
                    {
                        // In the unlikely event of a race condition, refresh and retry
                        await _context.Entry(counter).ReloadAsync();
                        counter.CurrentNumber++;
                        counter.LastUpdated = DateTime.UtcNow;
                        await _context.SaveChangesAsync();
                    }
                }
                else
                {
                    // Increment the existing counter
                    counter.CurrentNumber++;
                    counter.LastUpdated = DateTime.UtcNow;
                    await _context.SaveChangesAsync();
                }

                // Commit transaction
                await transaction.CommitAsync();

                // Format ticket number as T followed by 5 digits (e.g., T00001, T00002, etc.)
                return FormatTicketNumber(counter.CurrentNumber);
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
        finally
        {
            _counterLock.Release();
        }
    }
    
    private static string FormatTicketNumber(long ticketNumber)
    {
        return $"T{ticketNumber:D5}";
    }
}
