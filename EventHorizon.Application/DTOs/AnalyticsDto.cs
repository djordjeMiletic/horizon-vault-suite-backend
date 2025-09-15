namespace EventHorizon.Application.DTOs;

public record SeriesPoint(
    string Month,
    decimal Value
);

public record ProductMixItem(
    string Product,
    decimal Amount
);