using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QMS.DAL;
using QMS.DTO;

namespace QMS.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TicketsController : ControllerBase
{
    private readonly ILogger<TicketsController> _logger;
    private readonly ITicketRepository _ticketRepository;
    public TicketsController(ILogger<TicketsController> logger, ITicketRepository ticketRepository)
    {
        _logger = logger;
        _ticketRepository = ticketRepository;
    }

    [HttpPost("Create")]
    public async Task<IActionResult> CreateTicket()
    {

        var ticket = new Ticket
        {
        };

        ticket.IssuedAt = DateTime.UtcNow;
        ticket.IPAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
        ticket.TicketNumber = Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper(); // Generate a random ticket number
        await _ticketRepository.AddTicket(ticket);
        _logger.LogInformation($"Ticket created with ID: {ticket.Id}");
        return Ok(ticket); // Return the created ticket
        //return CreatedAtAction(nameof(CreateTicket), new { id = ticket.Id }, ticket);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllTickets()
    {
        var tickets = await _ticketRepository.GetAllTickets();
        return Ok(tickets);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetTicketById(Guid id)
    {
        var ticket = await _ticketRepository.GetTicketById(id);
        if (ticket == null)
        {
            return NotFound();
        }
        return Ok(ticket);
    }

    [HttpGet("Status/{status}")]
    public async Task<IActionResult> GetTicketsByStatus(TicketStatus status)
    {
        var tickets = await _ticketRepository.GetTicketsByStatus(status);
        return Ok(tickets);
    }

    [HttpPut("{id}/Status")]
    public async Task<IActionResult> UpdateTicketStatus(Guid id, [FromBody] TicketStatus status)
    {
        var ticket = await _ticketRepository.GetTicketById(id);
        if (ticket == null)
        {
            return NotFound();
        }
        await _ticketRepository.UpdateTicketStatus(id, status);
        _logger.LogInformation($"Ticket {id} status updated to {status}");
        return NoContent(); // Return 204 No Content on successful update
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTicket(Guid id)
    {
        var ticket = await _ticketRepository.GetTicketById(id);
        if (ticket == null)
        {
            return NotFound();
        }
        await _ticketRepository.DeleteTicket(id);
        _logger.LogInformation($"Ticket {id} deleted");
        return NoContent(); // Return 204 No Content on successful deletion
    }

    [HttpPost("AcquireTicket")]
    public async Task<IActionResult> AcquireTicket([FromBody] TicketAcquisitionRequest req)
    {
        if (string.IsNullOrEmpty(req.DeviceId))
        {
            return BadRequest("Device ID is required.");
        }

        var ticket = await _ticketRepository.GetNextAvailableTicket(req.DeviceId);
        
        if (ticket == null)
        {
            _logger.LogInformation("No ticket available at this time");
            return NotFound("No available tickets.");
        }
        _logger.LogInformation("Terminal {tId} has acquired {ticketInfo}", req.DeviceId, ticket.TicketNumber);



        return Ok(ticket);
    }
}
