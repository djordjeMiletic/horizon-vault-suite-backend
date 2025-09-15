using EventHorizon.Application.Common;
using EventHorizon.Application.DTOs;
using EventHorizon.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventHorizon.Api.Controllers;

/// <summary>
/// HR and Recruitment management
/// </summary>
[ApiController]
[Route("hr")]
[Authorize]
public class HRController : ControllerBase
{
    private readonly HRService _hrService;

    public HRController(HRService hrService)
    {
        _hrService = hrService;
    }

    // Jobs (Internal HR)
    /// <summary>
    /// Get internal HR jobs
    /// </summary>
    [HttpGet("jobs")]
    [Authorize(Policy = "Manager")]
    public async Task<ActionResult<PaginatedResult<JobPostingDto>>> GetJobs(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 12)
    {
        var jobs = await _hrService.GetJobsAsync(page, pageSize);
        return Ok(jobs);
    }

    // Applications
    /// <summary>
    /// Get job applications
    /// </summary>
    [HttpGet("applications")]
    [Authorize(Policy = "Manager")]
    public async Task<ActionResult<PaginatedResult<JobApplicationDto>>> GetApplications(
        [FromQuery] string? status = null,
        [FromQuery] Guid? jobId = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 12)
    {
        var applications = await _hrService.GetApplicationsAsync(status, jobId, page, pageSize);
        return Ok(applications);
    }

    /// <summary>
    /// Get application by ID
    /// </summary>
    [HttpGet("applications/{id}")]
    [Authorize(Policy = "Manager")]
    public async Task<ActionResult<JobApplicationDto>> GetApplication(Guid id)
    {
        var application = await _hrService.GetApplicationAsync(id);
        if (application == null) return NotFound();
        return Ok(application);
    }

    /// <summary>
    /// Update application status
    /// </summary>
    [HttpPatch("applications/{id}/status")]
    [Authorize(Policy = "Manager")]
    public async Task<IActionResult> UpdateApplicationStatus(Guid id, [FromBody] object statusUpdate)
    {
        // Extract status from the request body
        var status = statusUpdate.GetType().GetProperty("status")?.GetValue(statusUpdate)?.ToString();
        if (string.IsNullOrEmpty(status)) return BadRequest("Status is required");

        var success = await _hrService.UpdateApplicationStatusAsync(id, status);
        if (!success) return NotFound();
        return NoContent();
    }

    // Interviews
    /// <summary>
    /// Get interviews
    /// </summary>
    [HttpGet("interviews")]
    [Authorize(Policy = "Manager")]
    public async Task<ActionResult<IEnumerable<InterviewDto>>> GetInterviews(
        [FromQuery] string? candidateEmail = null,
        [FromQuery] Guid? jobId = null)
    {
        var interviews = await _hrService.GetInterviewsAsync(candidateEmail, jobId);
        return Ok(interviews);
    }

    /// <summary>
    /// Create interview
    /// </summary>
    [HttpPost("interviews")]
    [Authorize(Policy = "Manager")]
    public async Task<ActionResult<InterviewDto>> CreateInterview([FromBody] InterviewCreateDto dto)
    {
        var interview = await _hrService.CreateInterviewAsync(dto);
        return CreatedAtAction(nameof(GetInterviews), interview);
    }

    /// <summary>
    /// Update interview
    /// </summary>
    [HttpPut("interviews/{id}")]
    [Authorize(Policy = "Manager")]
    public async Task<ActionResult<InterviewDto>> UpdateInterview(Guid id, [FromBody] InterviewUpdateDto dto)
    {
        var interview = await _hrService.UpdateInterviewAsync(id, dto);
        if (interview == null) return NotFound();
        return Ok(interview);
    }

    /// <summary>
    /// Update interview status
    /// </summary>
    [HttpPatch("interviews/{id}/status")]
    [Authorize(Policy = "Manager")]
    public async Task<IActionResult> UpdateInterviewStatus(Guid id, [FromBody] object statusUpdate)
    {
        var status = statusUpdate.GetType().GetProperty("status")?.GetValue(statusUpdate)?.ToString();
        if (string.IsNullOrEmpty(status)) return BadRequest("Status is required");

        var success = await _hrService.UpdateInterviewStatusAsync(id, status);
        if (!success) return NotFound();
        return NoContent();
    }

    /// <summary>
    /// Delete interview
    /// </summary>
    [HttpDelete("interviews/{id}")]
    [Authorize(Policy = "Manager")]
    public async Task<IActionResult> DeleteInterview(Guid id)
    {
        var success = await _hrService.DeleteInterviewAsync(id);
        if (!success) return NotFound();
        return NoContent();
    }
}