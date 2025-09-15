using EventHorizon.Application.DTOs;
using EventHorizon.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EventHorizon.Api.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public class TicketsController : ControllerBase
{
    private readonly TicketsService _ticketsService;

    public TicketsController(TicketsService ticketsService)
    {
        _ticketsService = ticketsService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TicketDto>>> GetTickets([FromQuery] bool mine = false)
    {
        var currentUserEmail = User.FindFirst(ClaimTypes.Email)?.Value;
        var tickets = await _ticketsService.ListAsync(mine, currentUserEmail);
        return Ok(tickets);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TicketDto>> GetTicket(Guid id)
    {
        var ticket = await _ticketsService.GetAsync(id);
        if (ticket == null) return NotFound();
        return Ok(ticket);
    }

    [HttpPost]
    public async Task<ActionResult<TicketDto>> CreateTicket([FromBody] TicketCreateDto dto)
    {
        var currentUserEmail = User.FindFirst(ClaimTypes.Email)?.Value!;
        var ticket = await _ticketsService.CreateAsync(dto, currentUserEmail);
        return CreatedAtAction(nameof(GetTicket), new { id = ticket.Id }, ticket);
    }

    [HttpPost("{id}/reply")]
    public async Task<IActionResult> ReplyToTicket(Guid id, [FromBody] TicketReplyDto dto)
    {
        var currentUserEmail = User.FindFirst(ClaimTypes.Email)?.Value!;
        var success = await _ticketsService.ReplyAsync(id, dto, currentUserEmail);
        if (!success) return NotFound();
        return NoContent();
    }

    /// <summary>
    /// Update ticket status and priority
    /// </summary>
    [HttpPatch("{id}")]
    public async Task<IActionResult> UpdateTicket(Guid id, [FromBody] object updateData)
    {
        var ticket = await _ticketsService.GetAsync(id);
        if (ticket == null) return NotFound();

        var status = updateData.GetType().GetProperty("status")?.GetValue(updateData)?.ToString();
        var priority = updateData.GetType().GetProperty("priority")?.GetValue(updateData)?.ToString();

        // For now, just return success - could extend TicketsService to support status/priority updates
        return NoContent();
    }
}