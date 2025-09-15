using EventHorizon.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EventHorizon.Api.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public class AnalyticsController : ControllerBase
{
    private readonly AnalyticsService _analyticsService;

    public AnalyticsController(AnalyticsService analyticsService)
    {
        _analyticsService = analyticsService;
    }

    [HttpGet("series")]
    public async Task<ActionResult<Dictionary<string, decimal>>> GetSeries(
        [FromQuery] string range = "last6",
        [FromQuery] string? advisorEmail = null)
    {
        // Apply role-based filtering
        var currentUserEmail = User.FindFirst(ClaimTypes.Email)?.Value;
        var currentUserRole = User.FindFirst(ClaimTypes.Role)?.Value;

        if (currentUserRole == "Advisor")
        {
            advisorEmail = currentUserEmail; // Advisors can only see their own data
        }

        var series = await _analyticsService.GetSeriesAsync(range, advisorEmail);
        return Ok(series);
    }

    [HttpGet("product-mix")]
    public async Task<ActionResult<Dictionary<string, decimal>>> GetProductMix(
        [FromQuery] string range = "last6",
        [FromQuery] string? advisorEmail = null)
    {
        // Apply role-based filtering
        var currentUserEmail = User.FindFirst(ClaimTypes.Email)?.Value;
        var currentUserRole = User.FindFirst(ClaimTypes.Role)?.Value;

        if (currentUserRole == "Advisor")
        {
            advisorEmail = currentUserEmail; // Advisors can only see their own data
        }

        var productMix = await _analyticsService.GetProductMixAsync(range, advisorEmail);
        return Ok(productMix);
    }
}