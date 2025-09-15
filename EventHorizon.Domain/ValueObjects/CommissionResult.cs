namespace EventHorizon.Domain.ValueObjects;

public record CommissionResult(
    string MethodUsed,
    decimal ProductRatePct,
    decimal MarginPct,
    decimal CommissionBase,
    decimal PoolAmount,
    decimal AdvisorShare,
    decimal IntroducerShare,
    decimal ManagerShare,
    decimal ExecSalesManagerShare
);