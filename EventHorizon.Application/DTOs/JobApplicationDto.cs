namespace EventHorizon.Application.DTOs;

public record JobApplicationDto(
    Guid Id,
    Guid JobPostingId,
    string ApplicantName,
    string ApplicantEmail,
    string? CvPath,
    string? CoverLetter,
    DateTime CreatedAt,
    string Status,
    string JobTitle
);

public record JobApplicationCreateDto(
    Guid JobPostingId,
    string ApplicantName,
    string ApplicantEmail,
    string? CoverLetter
);

public record JobApplicationUpdateDto(
    string Status,
    string? Notes
);