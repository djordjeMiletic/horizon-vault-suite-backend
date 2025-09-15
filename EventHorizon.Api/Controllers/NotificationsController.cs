using EventHorizon.Application.DTOs;
using EventHorizon.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EventHorizon.Api.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public class NotificationsController : ControllerBase
{
    private readonly NotificationsService _notificationsService;

    public NotificationsController(NotificationsService notificationsService)
    {
        _notificationsService = notificationsService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<NotificationDto>>> GetNotifications(
        [FromQuery] string scope = "current")
    {
        var currentUserEmail = User.FindFirst(ClaimTypes.Email)?.Value;
        var notifications = await _notificationsService.ListAsync(scope, currentUserEmail);
        return Ok(notifications);
    }

    [HttpPost("{id}/read")]
    public async Task<IActionResult> MarkRead(Guid id)
    {
        var success = await _notificationsService.MarkReadAsync(id);
        if (!success) return NotFound();
        return NoContent();
    }

    [HttpPost("read-all")]
    public async Task<IActionResult> MarkAllRead([FromQuery] string? scope = null)
    {
        var currentUserEmail = User.FindFirst(ClaimTypes.Email)?.Value;
        await _notificationsService.MarkAllReadAsync(currentUserEmail, scope);
        return NoContent();
    }

    /// <summary>
    /// Mark all notifications as read (JSON body version)
    /// </summary>
    [HttpPost("read-all")]
    public async Task<IActionResult> MarkAllReadJson([FromBody] object scopeData)
    {
        var currentUserEmail = User.FindFirst(ClaimTypes.Email)?.Value;
        var scope = scopeData.GetType().GetProperty("scope")?.GetValue(scopeData)?.ToString();
        
        await _notificationsService.MarkAllReadAsync(currentUserEmail, scope);
        return NoContent();
    }
}