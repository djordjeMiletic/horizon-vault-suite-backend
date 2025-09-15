using EventHorizon.Application.Services;
using EventHorizon.Application.DTOs;
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
        [FromQuery] string? advisorEmail = null,
        [FromQuery] string format = "array")
    {
        // Apply role-based filtering
        var currentUserEmail = User.FindFirst(ClaimTypes.Email)?.Value;
        var currentUserRole = User.FindFirst(ClaimTypes.Role)?.Value;

        if (currentUserRole == "Advisor")
        {
            advisorEmail = currentUserEmail; // Advisors can only see their own data
        }

        if (format == "array")
        {
            var seriesArray = await _analyticsService.GetSeriesArrayAsync(range, advisorEmail);
            return Ok(seriesArray);
        }
        else
        {
            var series = await _analyticsService.GetSeriesAsync(range, advisorEmail);
            return Ok(series);
        }
    }

    /// <summary>
    /// Get analytics series data as array format
    /// </summary>
    [HttpGet("series")]
    public async Task<ActionResult<SeriesPoint[]>> GetSeriesArray(
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

        var series = await _analyticsService.GetSeriesArrayAsync(range, advisorEmail);
        return Ok(series);
    }

    [HttpGet("product-mix")]
    public async Task<ActionResult<Dictionary<string, decimal>>> GetProductMix(
        [FromQuery] string range = "last6",
        [FromQuery] string? advisorEmail = null,
        [FromQuery] string format = "array")
    {
        // Apply role-based filtering
        var currentUserEmail = User.FindFirst(ClaimTypes.Email)?.Value;
        var currentUserRole = User.FindFirst(ClaimTypes.Role)?.Value;

        if (currentUserRole == "Advisor")
        {
            advisorEmail = currentUserEmail; // Advisors can only see their own data
        }

        if (format == "array")
        {
            var productMixArray = await _analyticsService.GetProductMixArrayAsync(range, advisorEmail);
            return Ok(productMixArray);
        }
        else
        {
            var productMix = await _analyticsService.GetProductMixAsync(range, advisorEmail);
            return Ok(productMix);
        }
    }

    /// <summary>
    /// Get product mix data as array format
    /// </summary>
    [HttpGet("product-mix")]
    public async Task<ActionResult<ProductMixItem[]>> GetProductMixArray(
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

        var productMix = await _analyticsService.GetProductMixArrayAsync(range, advisorEmail);
        return Ok(productMix);
    }
}