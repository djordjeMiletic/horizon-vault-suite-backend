using EventHorizon.Application.DTOs;
using EventHorizon.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventHorizon.Api.Controllers;

/// <summary>
/// Onboarding tasks management
/// </summary>
[ApiController]
[Route("onboarding")]
[Authorize]
public class OnboardingController : ControllerBase
{
    private readonly HRService _hrService;

    public OnboardingController(HRService hrService)
    {
        _hrService = hrService;
    }

    /// <summary>
    /// Get onboarding tasks
    /// </summary>
    [HttpGet("tasks")]
    [Authorize(Policy = "Manager")]
    public async Task<ActionResult<IEnumerable<OnboardingTaskDto>>> GetTasks(
        [FromQuery] string? assigneeEmail = null)
    {
        var tasks = await _hrService.GetOnboardingTasksAsync(assigneeEmail);
        return Ok(tasks);
    }

    /// <summary>
    /// Create onboarding task
    /// </summary>
    [HttpPost("tasks")]
    [Authorize(Policy = "Manager")]
    public async Task<ActionResult<OnboardingTaskDto>> CreateTask([FromBody] OnboardingTaskCreateDto dto)
    {
        var task = await _hrService.CreateOnboardingTaskAsync(dto);
        return CreatedAtAction(nameof(GetTasks), task);
    }

    /// <summary>
    /// Update onboarding task
    /// </summary>
    [HttpPut("tasks/{id}")]
    [Authorize(Policy = "Manager")]
    public async Task<ActionResult<OnboardingTaskDto>> UpdateTask(Guid id, [FromBody] OnboardingTaskUpdateDto dto)
    {
        var task = await _hrService.UpdateOnboardingTaskAsync(id, dto);
        if (task == null) return NotFound();
        return Ok(task);
    }

    /// <summary>
    /// Complete onboarding task
    /// </summary>
    [HttpPut("tasks/{id}/complete")]
    public async Task<IActionResult> CompleteTask(Guid id)
    {
        var success = await _hrService.CompleteOnboardingTaskAsync(id);
        if (!success) return NotFound();
        return NoContent();
    }

    /// <summary>
    /// Delete onboarding task
    /// </summary>
    [HttpDelete("tasks/{id}")]
    [Authorize(Policy = "Manager")]
    public async Task<IActionResult> DeleteTask(Guid id)
    {
        var success = await _hrService.DeleteOnboardingTaskAsync(id);
        if (!success) return NotFound();
        return NoContent();
    }
}