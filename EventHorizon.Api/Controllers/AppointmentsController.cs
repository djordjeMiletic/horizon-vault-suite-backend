using EventHorizon.Application.DTOs;
using EventHorizon.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EventHorizon.Api.Controllers;

/// <summary>
/// Appointments management
/// </summary>
[ApiController]
[Route("[controller]")]
[Route("appointments")] // Lowercase alias
[Authorize]
public class AppointmentsController : ControllerBase
{
    private readonly ClientService _clientService;

    public AppointmentsController(ClientService clientService)
    {
        _clientService = clientService;
    }

    /// <summary>
    /// Get appointments
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<AppointmentDto>>> GetAppointments(
        [FromQuery] string? ownerEmail = null,
        [FromQuery] string? advisorEmail = null,
        [FromQuery] DateOnly? from = null,
        [FromQuery] DateOnly? to = null)
    {
        var currentUserEmail = User.FindFirst(ClaimTypes.Email)?.Value;
        var currentUserRole = User.FindFirst(ClaimTypes.Role)?.Value;

        // Apply role-based filtering
        if (currentUserRole == "Client")
        {
            ownerEmail = currentUserEmail; // Clients can only see their own appointments
        }
        else if (currentUserRole == "Advisor")
        {
            advisorEmail = currentUserEmail; // Advisors can only see their own appointments
        }

        var appointments = await _clientService.GetAppointmentsAsync(ownerEmail, advisorEmail, from, to);
        return Ok(appointments);
    }

    /// <summary>
    /// Create appointment
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<AppointmentDto>> CreateAppointment([FromBody] AppointmentCreateDto dto)
    {
        var appointment = await _clientService.CreateAppointmentAsync(dto);
        return CreatedAtAction(nameof(GetAppointments), appointment);
    }

    /// <summary>
    /// Update appointment
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult<AppointmentDto>> UpdateAppointment(Guid id, [FromBody] AppointmentUpdateDto dto)
    {
        var appointment = await _clientService.UpdateAppointmentAsync(id, dto);
        if (appointment == null) return NotFound();
        return Ok(appointment);
    }

    /// <summary>
    /// Delete appointment
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAppointment(Guid id)
    {
        var success = await _clientService.DeleteAppointmentAsync(id);
        if (!success) return NotFound();
        return NoContent();
    }
}