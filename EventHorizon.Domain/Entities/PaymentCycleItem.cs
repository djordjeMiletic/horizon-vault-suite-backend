namespace EventHorizon.Domain.Entities;

public class PaymentCycleItem
{
    public Guid Id { get; set; }
    public Guid PaymentId { get; set; }
    public Guid CycleId { get; set; }
    public string ProposedStatus { get; set; } = string.Empty;
    public string FinalStatus { get; set; } = "None";
    public string? ManagerNote { get; set; }
    public DateTime UpdatedAt { get; set; }

    public Payment Payment { get; set; } = null!;
    public PaymentCycle Cycle { get; set; } = null!;
}