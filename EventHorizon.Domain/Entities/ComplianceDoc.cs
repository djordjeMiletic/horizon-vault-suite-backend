namespace EventHorizon.Domain.Entities;

public class ComplianceDoc
{
    public Guid Id { get; set; }
    public string OwnerEmail { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Status { get; set; } = "Draft"; // "Draft", "Under Review", "Approved", "Rejected"
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public string? Description { get; set; }
    public Guid? DocumentId { get; set; }

    public Document? Document { get; set; }
}