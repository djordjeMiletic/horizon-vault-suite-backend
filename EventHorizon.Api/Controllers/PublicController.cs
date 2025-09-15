using EventHorizon.Application.DTOs;
using EventHorizon.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace EventHorizon.Api.Controllers;

[ApiController]
[Route("public")]
public class PublicController : ControllerBase
{
    private readonly PublicFormsService _publicFormsService;

    public PublicController(PublicFormsService publicFormsService)
    {
        _publicFormsService = publicFormsService;
    }

    [HttpPost("jobs/apply")]
    public async Task<ActionResult<Guid>> ApplyForJob([FromForm] JobApplicationCreateDto dto)
    {
        var applicationId = await _publicFormsService.CreateJobApplicationAsync(dto);
        return Ok(new { applicationId });
    }

    [HttpPost("inquiries")]
    public async Task<ActionResult<Guid>> CreateInquiry([FromBody] WebsiteInquiryCreateDto dto)
    {
        var inquiryId = await _publicFormsService.CreateInquiryAsync(dto);
        return Ok(new { inquiryId });
    }

    [HttpGet("jobs")]
    public async Task<ActionResult<IEnumerable<JobPostingDto>>> GetOpenJobs()
    {
        var jobs = await _publicFormsService.ListOpenJobPostingsAsync();
        return Ok(jobs);
    }

    // Route aliases for frontend compatibility
    /// <summary>
    /// Get open jobs (alias)
    /// </summary>
    [HttpGet("/jobs/public")]
    public async Task<ActionResult<IEnumerable<JobPostingDto>>> GetOpenJobsAlias()
    {
        return await GetOpenJobs();
    }

    /// <summary>
    /// Create inquiry (alias)
    /// </summary>
    [HttpPost("/inquiries")]
    public async Task<ActionResult<Guid>> CreateInquiryAlias([FromBody] WebsiteInquiryCreateDto dto)
    {
        return await CreateInquiry(dto);
    }
}