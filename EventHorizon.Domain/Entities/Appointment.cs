namespace EventHorizon.Domain.Entities;

public class Appointment
{
    public Guid Id { get; set; }
    public string ClientEmail { get; set; } = string.Empty;
    public string AdvisorEmail { get; set; } = string.Empty;
    public DateTime StartAt { get; set; }
    public DateTime EndAt { get; set; }
    public string Location { get; set; } = string.Empty;
    public string Status { get; set; } = "Scheduled"; // "Scheduled", "Completed", "Cancelled", "No-Show"
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
}