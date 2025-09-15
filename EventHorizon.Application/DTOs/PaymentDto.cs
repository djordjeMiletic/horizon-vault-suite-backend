using EventHorizon.Domain.ValueObjects;

namespace EventHorizon.Application.DTOs;

public class PaymentDto
{
    public Guid Id { get; set; }
    public DateOnly Date { get; set; }
    public string AdvisorEmail { get; set; } = string.Empty;
    public Guid ProductId { get; set; }
    public string Provider { get; set; } = string.Empty;
    public decimal APE { get; set; }
    public decimal Receipts { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public string ProductName { get; set; } = string.Empty;
}

public class PaymentCreateDto
{
    public DateOnly Date { get; set; }
    public Guid ProductId { get; set; }
    public string Provider { get; set; } = string.Empty;
    public decimal APE { get; set; }
    public decimal Receipts { get; set; }
    public string? Notes { get; set; }
    public string AdvisorEmail { get; set; } = string.Empty;
}

public class PaymentWithCommissionDto
{
    public PaymentDto Payment { get; set; } = null!;
    public CommissionResult Commission { get; set; } = null!;
}

public class CommissionDetailsRowDto
{
    public DateOnly Date { get; set; }
    public string Provider { get; set; } = string.Empty;
    public string Product { get; set; } = string.Empty;
    public string MethodUsed { get; set; } = string.Empty;
    public decimal ProductRatePct { get; set; }
    public decimal MarginPct { get; set; }
    public decimal CommissionBase { get; set; }
    public decimal PoolAmount { get; set; }
    public decimal AdvisorShare { get; set; }
    public decimal IntroducerShare { get; set; }
    public decimal ManagerShare { get; set; }
    public decimal ExecShare { get; set; }
}