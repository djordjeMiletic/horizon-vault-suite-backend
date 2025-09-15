using EventHorizon.Application.DTOs;
using EventHorizon.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventHorizon.Api.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public class PoliciesController : ControllerBase
{
    private readonly PoliciesService _policiesService;

    public PoliciesController(PoliciesService policiesService)
    {
        _policiesService = policiesService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PolicyDto>>> GetPolicies()
    {
        var policies = await _policiesService.GetAllAsync();
        return Ok(policies);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<PolicyDto>> GetPolicy(Guid id)
    {
        var policy = await _policiesService.GetByIdAsync(id);
        if (policy == null) return NotFound();
        return Ok(policy);
    }

    [HttpPost]
    [Authorize(Policy = "Administrator")]
    public async Task<ActionResult<PolicyDto>> CreatePolicy([FromBody] PolicyCreateDto dto)
    {
        var policy = await _policiesService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetPolicy), new { id = policy.Id }, policy);
    }

    [HttpPut("{id}")]
    [Authorize(Policy = "Administrator")]
    public async Task<ActionResult<PolicyDto>> UpdatePolicy(Guid id, [FromBody] PolicyUpdateDto dto)
    {
        var policy = await _policiesService.UpdateAsync(id, dto);
        if (policy == null) return NotFound();
        return Ok(policy);
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = "Administrator")]
    public async Task<IActionResult> DeletePolicy(Guid id)
    {
        var success = await _policiesService.DeleteAsync(id);
        if (!success) return NotFound();
        return NoContent();
    }
}