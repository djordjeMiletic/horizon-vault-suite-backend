namespace EventHorizon.Domain.Entities;

public class OnboardingTask
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string AssigneeEmail { get; set; } = string.Empty;
    public string Status { get; set; } = "Pending"; // "Pending", "In Progress", "Completed"
    public DateTime CreatedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string? Description { get; set; }
}