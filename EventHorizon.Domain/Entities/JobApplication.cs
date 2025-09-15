namespace EventHorizon.Domain.Entities;

public class JobApplication
{
    public Guid Id { get; set; }
    public Guid JobPostingId { get; set; }
    public string ApplicantName { get; set; } = string.Empty;
    public string ApplicantEmail { get; set; } = string.Empty;
    public string? CVPath { get; set; }
    public string? CoverLetter { get; set; }
    public DateTime CreatedAt { get; set; }
    public string Status { get; set; } = "New";

    public JobPosting JobPosting { get; set; } = null!;
}