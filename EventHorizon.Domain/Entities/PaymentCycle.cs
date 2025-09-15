namespace EventHorizon.Domain.Entities;

public class PaymentCycle
{
    public Guid Id { get; set; }
    public string CycleKey { get; set; } = string.Empty;
    public string Status { get; set; } = "Open";
    public DateTime CreatedAt { get; set; }
    public DateTime? ClosedAt { get; set; }

    public ICollection<PaymentCycleItem> Items { get; set; } = new List<PaymentCycleItem>();
}