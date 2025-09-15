namespace EventHorizon.Application.DTOs;

public record ComplianceDocDto(
    Guid Id,
    string OwnerEmail,
    string Title,
    string Status,
    DateTime CreatedAt,
    DateTime UpdatedAt
);

public record ComplianceDocCreateDto(
    string Title,
    string? Description
);

public record ComplianceDocUpdateDto(
    string Title,
    string? Description
);

public record AuditEntryDto(
    Guid Id,
    DateTime At,
    string Actor,
    string Action,
    string EntityType,
    string EntityId,
    string? Details
);