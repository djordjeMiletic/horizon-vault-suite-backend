using EventHorizon.Domain.Entities;
using EventHorizon.Domain.ValueObjects;

namespace EventHorizon.Domain.Interfaces;

public interface ICommissionCalculator
{
    CommissionResult Compute(Payment payment, Policy policy);
}