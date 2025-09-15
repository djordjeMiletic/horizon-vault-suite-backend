using EventHorizon.Application.Common;
using EventHorizon.Application.DTOs;
using EventHorizon.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventHorizon.Api.Controllers;

/// <summary>
/// Referral partners management
/// </summary>
[ApiController]
[Route("[controller]")]
[Route("referrals")] // Lowercase alias
[Authorize]
public class ReferralsController : ControllerBase
{
    private readonly CRMService _crmService;

    public ReferralsController(CRMService crmService)
    {
        _crmService = crmService;
    }

    /// <summary>
    /// Get referral partners
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<PaginatedResult<ReferralPartnerDto>>> GetReferrals(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 12)
    {
        var referrals = await _crmService.GetReferralPartnersAsync(page, pageSize);
        return Ok(referrals);
    }

    /// <summary>
    /// Create referral partner
    /// </summary>
    [HttpPost]
    [Authorize(Policy = "Manager")]
    public async Task<ActionResult<ReferralPartnerDto>> CreateReferral([FromBody] ReferralPartnerCreateDto dto)
    {
        var referral = await _crmService.CreateReferralPartnerAsync(dto);
        return CreatedAtAction(nameof(GetReferrals), referral);
    }

    /// <summary>
    /// Update referral partner
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Policy = "Manager")]
    public async Task<ActionResult<ReferralPartnerDto>> UpdateReferral(Guid id, [FromBody] ReferralPartnerUpdateDto dto)
    {
        var referral = await _crmService.UpdateReferralPartnerAsync(id, dto);
        if (referral == null) return NotFound();
        return Ok(referral);
    }

    /// <summary>
    /// Update referral partner status
    /// </summary>
    [HttpPatch("{id}/status")]
    [Authorize(Policy = "Manager")]
    public async Task<IActionResult> UpdateReferralStatus(Guid id, [FromBody] object statusUpdate)
    {
        var activeValue = statusUpdate.GetType().GetProperty("active")?.GetValue(statusUpdate);
        if (activeValue == null) return BadRequest("Active status is required");

        var active = Convert.ToBoolean(activeValue);
        var success = await _crmService.UpdateReferralPartnerStatusAsync(id, active);
        if (!success) return NotFound();
        return NoContent();
    }

    /// <summary>
    /// Delete referral partner
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Policy = "Manager")]
    public async Task<IActionResult> DeleteReferral(Guid id)
    {
        var success = await _crmService.DeleteReferralPartnerAsync(id);
        if (!success) return NotFound();
        return NoContent();
    }
}