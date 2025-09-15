namespace EventHorizon.Application.DTOs;

public record AppointmentDto(
    Guid Id,
    string ClientEmail,
    string AdvisorEmail,
    DateTime StartAt,
    DateTime EndAt,
    string Location,
    string Status,
    string? Notes
);

public record AppointmentCreateDto(
    string ClientEmail,
    string AdvisorEmail,
    DateTime StartAt,
    DateTime EndAt,
    string Location,
    string? Notes
);

public record AppointmentUpdateDto(
    string ClientEmail,
    string AdvisorEmail,
    DateTime StartAt,
    DateTime EndAt,
    string Location,
    string? Notes
);