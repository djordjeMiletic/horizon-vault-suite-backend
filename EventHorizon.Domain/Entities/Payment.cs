namespace EventHorizon.Domain.Entities;

public class Payment
{
    public Guid Id { get; set; }
    public DateOnly Date { get; set; }
    public string AdvisorEmail { get; set; } = string.Empty;
    public Guid ProductId { get; set; }
    public string Provider { get; set; } = string.Empty;
    public decimal APE { get; set; }
    public decimal Receipts { get; set; }
    public string Status { get; set; } = "Pending";
    public string? Notes { get; set; }

    public Policy Product { get; set; } = null!;
    public ICollection<PaymentCycleItem> CycleItems { get; set; } = new List<PaymentCycleItem>();
}