using EventHorizon.Application.Common;
using EventHorizon.Application.DTOs;
using EventHorizon.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EventHorizon.Api.Controllers;

/// <summary>
/// Compliance documents management
/// </summary>
[ApiController]
[Route("[controller]")]
[Route("compliance")] // Lowercase alias
[Authorize]
public class ComplianceController : ControllerBase
{
    private readonly ComplianceService _complianceService;

    public ComplianceController(ComplianceService complianceService)
    {
        _complianceService = complianceService;
    }

    /// <summary>
    /// Get compliance documents
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<PaginatedResult<ComplianceDocDto>>> GetComplianceDocs(
        [FromQuery] string? status = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 12)
    {
        var docs = await _complianceService.GetComplianceDocsAsync(status, page, pageSize);
        return Ok(docs);
    }

    /// <summary>
    /// Create compliance document
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ComplianceDocDto>> CreateComplianceDoc([FromBody] ComplianceDocCreateDto dto)
    {
        var currentUserEmail = User.FindFirst(ClaimTypes.Email)?.Value!;
        var doc = await _complianceService.CreateComplianceDocAsync(dto, currentUserEmail);
        return CreatedAtAction(nameof(GetComplianceDocs), doc);
    }

    /// <summary>
    /// Update compliance document
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult<ComplianceDocDto>> UpdateComplianceDoc(Guid id, [FromBody] ComplianceDocUpdateDto dto)
    {
        var doc = await _complianceService.UpdateComplianceDocAsync(id, dto);
        if (doc == null) return NotFound();
        return Ok(doc);
    }

    /// <summary>
    /// Delete compliance document
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Policy = "Administrator")]
    public async Task<IActionResult> DeleteComplianceDoc(Guid id)
    {
        var success = await _complianceService.DeleteComplianceDocAsync(id);
        if (!success) return NotFound();
        return NoContent();
    }
}