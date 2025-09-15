namespace EventHorizon.Application.DTOs;

public record LeadDto(
    Guid Id,
    string Name,
    string Email,
    string Phone,
    string Status,
    string OwnerEmail,
    DateTime CreatedAt
);

public record LeadCreateDto(
    string Name,
    string Email,
    string Phone,
    string OwnerEmail,
    string? Notes
);

public record LeadUpdateDto(
    string Name,
    string Email,
    string Phone,
    string? Notes
);

public record PipelineDealDto(
    Guid Id,
    string Title,
    string AdvisorEmail,
    string Stage,
    decimal Value,
    DateTime UpdatedAt
);

public record PipelineDealCreateDto(
    string Title,
    string AdvisorEmail,
    string Stage,
    decimal Value,
    string? Notes
);

public record PipelineDealUpdateDto(
    string Title,
    string Stage,
    decimal Value,
    string? Notes
);

public record ReferralPartnerDto(
    Guid Id,
    string Name,
    string Email,
    string? Phone,
    bool Active,
    DateTime CreatedAt
);

public record ReferralPartnerCreateDto(
    string Name,
    string Email,
    string? Phone,
    string? Notes
);

public record ReferralPartnerUpdateDto(
    string Name,
    string Email,
    string? Phone,
    string? Notes
);