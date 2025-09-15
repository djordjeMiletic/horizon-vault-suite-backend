namespace EventHorizon.Domain.Entities;

public class PipelineDeal
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string AdvisorEmail { get; set; } = string.Empty;
    public string Stage { get; set; } = "Prospect"; // "Prospect", "Qualified", "Proposal", "Negotiation", "Closed Won", "Closed Lost"
    public decimal Value { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? Notes { get; set; }
}