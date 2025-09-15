using Microsoft.AspNetCore.Http;

namespace EventHorizon.Application.DTOs;

public class JobPostingDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsOpen { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class JobApplicationCreateDto
{
    public Guid JobPostingId { get; set; }
    public string ApplicantName { get; set; } = string.Empty;
    public string ApplicantEmail { get; set; } = string.Empty;
    public IFormFile? CV { get; set; }
    public string? CoverLetter { get; set; }
}

public class WebsiteInquiryCreateDto
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string? Source { get; set; }
}