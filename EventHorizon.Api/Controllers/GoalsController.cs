using EventHorizon.Application.DTOs;
using EventHorizon.Application.Services;
using EventHorizon.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EventHorizon.Api.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public class GoalsController : ControllerBase
{
    private readonly GoalsService _goalsService;

    public GoalsController(GoalsService goalsService)
    {
        _goalsService = goalsService;
    }

    [HttpGet]
    public async Task<ActionResult<GoalDto>> GetGoals(
        [FromQuery] string subject = "advisor",
        [FromQuery] string? @ref = null)
    {
        var currentUserEmail = User.FindFirst(ClaimTypes.Email)?.Value;
        var currentUserRole = User.FindFirst(ClaimTypes.Role)?.Value;

        // Parse subject type
        if (!Enum.TryParse<SubjectType>(subject, true, out var subjectType))
            return BadRequest("Invalid subject type");

        // Determine reference based on role and parameters
        string subjectRef;
        if (currentUserRole == "Advisor")
        {
            subjectRef = currentUserEmail!; // Advisors can only see their own goals
        }
        else if (subjectType == SubjectType.Manager && string.IsNullOrEmpty(@ref))
        {
            subjectRef = "team"; // Default to team for managers
        }
        else
        {
            subjectRef = @ref ?? currentUserEmail!;
        }

        var goals = await _goalsService.GetGoalsAsync(subjectType, subjectRef);
        if (goals == null) return NotFound();

        return Ok(goals);
    }
}