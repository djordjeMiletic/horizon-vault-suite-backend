namespace EventHorizon.Application.DTOs;

public record InterviewDto(
    Guid Id,
    Guid JobApplicationId,
    DateTime ScheduledAt,
    string Mode,
    string Status,
    string? Notes
);

public record InterviewCreateDto(
    Guid JobApplicationId,
    DateTime ScheduledAt,
    string Mode,
    string? Notes
);

public record InterviewUpdateDto(
    DateTime ScheduledAt,
    string Mode,
    string? Notes
);

public record OnboardingTaskDto(
    Guid Id,
    string Title,
    string AssigneeEmail,
    string Status,
    DateTime CreatedAt,
    DateTime? CompletedAt
);

public record OnboardingTaskCreateDto(
    string Title,
    string AssigneeEmail,
    string? Description
);

public record OnboardingTaskUpdateDto(
    string Title,
    string AssigneeEmail,
    string? Description
);