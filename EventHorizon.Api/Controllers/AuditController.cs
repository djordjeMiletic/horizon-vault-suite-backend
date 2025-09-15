using EventHorizon.Application.Common;
using EventHorizon.Application.DTOs;
using EventHorizon.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventHorizon.Api.Controllers;

/// <summary>
/// Audit trail management
/// </summary>
[ApiController]
[Route("[controller]")]
[Route("audit")] // Lowercase alias
[Authorize(Policy = "Administrator")]
public class AuditController : ControllerBase
{
    private readonly AuditService _auditService;

    public AuditController(AuditService auditService)
    {
        _auditService = auditService;
    }

    /// <summary>
    /// Get audit entries
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<PaginatedResult<AuditEntryDto>>> GetAuditEntries(
        [FromQuery] string? entityType = null,
        [FromQuery] string? entityId = null,
        [FromQuery] string? actor = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 12)
    {
        var entries = await _auditService.GetAuditEntriesAsync(entityType, entityId, actor, page, pageSize);
        return Ok(entries);
    }

    /// <summary>
    /// Get audit entry by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<AuditEntryDto>> GetAuditEntry(Guid id)
    {
        var entry = await _auditService.GetAuditEntryAsync(id);
        if (entry == null) return NotFound();
        return Ok(entry);
    }
}