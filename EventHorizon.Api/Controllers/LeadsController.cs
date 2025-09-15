using EventHorizon.Application.Common;
using EventHorizon.Application.DTOs;
using EventHorizon.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EventHorizon.Api.Controllers;

/// <summary>
/// Leads management
/// </summary>
[ApiController]
[Route("[controller]")]
[Route("leads")] // Lowercase alias
[Authorize]
public class LeadsController : ControllerBase
{
    private readonly CRMService _crmService;

    public LeadsController(CRMService crmService)
    {
        _crmService = crmService;
    }

    /// <summary>
    /// Get leads
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<PaginatedResult<LeadDto>>> GetLeads(
        [FromQuery] string? status = null,
        [FromQuery] string? ownerEmail = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 12)
    {
        var currentUserEmail = User.FindFirst(ClaimTypes.Email)?.Value;
        var currentUserRole = User.FindFirst(ClaimTypes.Role)?.Value;

        // Apply role-based filtering
        if (currentUserRole == "Advisor")
        {
            ownerEmail = currentUserEmail; // Advisors can only see their own leads
        }

        var leads = await _crmService.GetLeadsAsync(status, ownerEmail, page, pageSize);
        return Ok(leads);
    }

    /// <summary>
    /// Create lead
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<LeadDto>> CreateLead([FromBody] LeadCreateDto dto)
    {
        var lead = await _crmService.CreateLeadAsync(dto);
        return CreatedAtAction(nameof(GetLeads), lead);
    }

    /// <summary>
    /// Update lead
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult<LeadDto>> UpdateLead(Guid id, [FromBody] LeadUpdateDto dto)
    {
        var lead = await _crmService.UpdateLeadAsync(id, dto);
        if (lead == null) return NotFound();
        return Ok(lead);
    }

    /// <summary>
    /// Assign lead to owner
    /// </summary>
    [HttpPatch("{id}/assign")]
    [Authorize(Policy = "Manager")]
    public async Task<IActionResult> AssignLead(Guid id, [FromBody] object assignUpdate)
    {
        var ownerEmail = assignUpdate.GetType().GetProperty("ownerEmail")?.GetValue(assignUpdate)?.ToString();
        if (string.IsNullOrEmpty(ownerEmail)) return BadRequest("Owner email is required");

        var success = await _crmService.AssignLeadAsync(id, ownerEmail);
        if (!success) return NotFound();
        return NoContent();
    }

    /// <summary>
    /// Update lead status
    /// </summary>
    [HttpPatch("{id}/status")]
    public async Task<IActionResult> UpdateLeadStatus(Guid id, [FromBody] object statusUpdate)
    {
        var status = statusUpdate.GetType().GetProperty("status")?.GetValue(statusUpdate)?.ToString();
        if (string.IsNullOrEmpty(status)) return BadRequest("Status is required");

        var success = await _crmService.UpdateLeadStatusAsync(id, status);
        if (!success) return NotFound();
        return NoContent();
    }

    /// <summary>
    /// Delete lead
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteLead(Guid id)
    {
        var success = await _crmService.DeleteLeadAsync(id);
        if (!success) return NotFound();
        return NoContent();
    }
}