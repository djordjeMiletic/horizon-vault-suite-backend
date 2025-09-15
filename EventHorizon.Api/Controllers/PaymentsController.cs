using EventHorizon.Application.Common;
using EventHorizon.Application.DTOs;
using EventHorizon.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EventHorizon.Api.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public class PaymentsController : ControllerBase
{
    private readonly PaymentsService _paymentsService;

    public PaymentsController(PaymentsService paymentsService)
    {
        _paymentsService = paymentsService;
    }

    [HttpGet]
    public async Task<ActionResult<PaginatedResult<PaymentDto>>> GetPayments(
        [FromQuery] DateOnly? from = null,
        [FromQuery] DateOnly? to = null,
        [FromQuery] string? advisorEmail = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 12)
    {
        // Apply role-based filtering
        var currentUserEmail = User.FindFirst(ClaimTypes.Email)?.Value;
        var currentUserRole = User.FindFirst(ClaimTypes.Role)?.Value;

        if (currentUserRole == "Advisor")
        {
            advisorEmail = currentUserEmail; // Advisors can only see their own payments
        }

        var payments = await _paymentsService.GetPaymentsAsync(from, to, advisorEmail, page, pageSize);
        return Ok(payments);
    }

    [HttpPost]
    public async Task<ActionResult<PaymentWithCommissionDto>> CreatePayment([FromBody] PaymentCreateDto dto)
    {
        var currentUserEmail = User.FindFirst(ClaimTypes.Email)?.Value;
        var currentUserRole = User.FindFirst(ClaimTypes.Role)?.Value;

        // Advisors can only create payments for themselves
        if (currentUserRole == "Advisor")
        {
            dto.AdvisorEmail = currentUserEmail!;
        }

        var payment = await _paymentsService.AddPaymentAsync(dto);
        return CreatedAtAction("GetPayments", payment);
    }
}