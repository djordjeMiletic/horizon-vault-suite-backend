namespace EventHorizon.Domain.Entities;

public class Interview
{
    public Guid Id { get; set; }
    public Guid JobApplicationId { get; set; }
    public DateTime ScheduledAt { get; set; }
    public string Mode { get; set; } = string.Empty; // "In-Person", "Video", "Phone"
    public string Status { get; set; } = "Scheduled"; // "Scheduled", "Completed", "Cancelled", "No-Show"
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }

    public JobApplication JobApplication { get; set; } = null!;
}