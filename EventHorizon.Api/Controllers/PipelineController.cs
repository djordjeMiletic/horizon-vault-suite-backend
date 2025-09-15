using EventHorizon.Application.Common;
using EventHorizon.Application.DTOs;
using EventHorizon.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EventHorizon.Api.Controllers;

/// <summary>
/// Sales pipeline management
/// </summary>
[ApiController]
[Route("[controller]")]
[Route("pipeline")] // Lowercase alias
[Authorize]
public class PipelineController : ControllerBase
{
    private readonly CRMService _crmService;

    public PipelineController(CRMService crmService)
    {
        _crmService = crmService;
    }

    /// <summary>
    /// Get pipeline deals
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<PaginatedResult<PipelineDealDto>>> GetPipeline(
        [FromQuery] string? stage = null,
        [FromQuery] string? advisorEmail = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 12)
    {
        var currentUserEmail = User.FindFirst(ClaimTypes.Email)?.Value;
        var currentUserRole = User.FindFirst(ClaimTypes.Role)?.Value;

        // Apply role-based filtering
        if (currentUserRole == "Advisor")
        {
            advisorEmail = currentUserEmail; // Advisors can only see their own deals
        }

        var pipeline = await _crmService.GetPipelineAsync(stage, advisorEmail, page, pageSize);
        return Ok(pipeline);
    }

    /// <summary>
    /// Create pipeline deal
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<PipelineDealDto>> CreateDeal([FromBody] PipelineDealCreateDto dto)
    {
        var deal = await _crmService.CreatePipelineDealAsync(dto);
        return CreatedAtAction(nameof(GetPipeline), deal);
    }

    /// <summary>
    /// Update pipeline deal
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult<PipelineDealDto>> UpdateDeal(Guid id, [FromBody] PipelineDealUpdateDto dto)
    {
        var deal = await _crmService.UpdatePipelineDealAsync(id, dto);
        if (deal == null) return NotFound();
        return Ok(deal);
    }

    /// <summary>
    /// Move deal to different stage
    /// </summary>
    [HttpPatch("{id}/move")]
    public async Task<IActionResult> MoveDeal(Guid id, [FromBody] object stageUpdate)
    {
        var stage = stageUpdate.GetType().GetProperty("stage")?.GetValue(stageUpdate)?.ToString();
        if (string.IsNullOrEmpty(stage)) return BadRequest("Stage is required");

        var success = await _crmService.MovePipelineDealAsync(id, stage);
        if (!success) return NotFound();
        return NoContent();
    }

    /// <summary>
    /// Delete pipeline deal
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteDeal(Guid id)
    {
        var success = await _crmService.DeletePipelineDealAsync(id);
        if (!success) return NotFound();
        return NoContent();
    }
}