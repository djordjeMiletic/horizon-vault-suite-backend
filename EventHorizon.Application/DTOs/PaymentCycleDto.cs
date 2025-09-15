namespace EventHorizon.Application.DTOs;

public class PaymentCycleDto
{
    public Guid Id { get; set; }
    public string CycleKey { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? ClosedAt { get; set; }
    public List<PaymentCycleItemDto> Items { get; set; } = new();
}

public class PaymentCycleCreateDto
{
    public string CycleKey { get; set; } = string.Empty;
    public string? Notes { get; set; }
}

public class PaymentCycleItemDto
{
    public Guid Id { get; set; }
    public Guid PaymentId { get; set; }
    public string ProposedStatus { get; set; } = string.Empty;
    public string FinalStatus { get; set; } = string.Empty;
    public string? ManagerNote { get; set; }
    public DateTime UpdatedAt { get; set; }
    public PaymentDto Payment { get; set; } = null!;
}

public class ProposedStatusDto
{
    public string Status { get; set; } = string.Empty;
    public string? Note { get; set; }
}

public class FinalStatusDto
{
    public string Status { get; set; } = string.Empty;
}