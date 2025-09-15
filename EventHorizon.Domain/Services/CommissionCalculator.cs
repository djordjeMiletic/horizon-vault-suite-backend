using EventHorizon.Domain.Entities;
using EventHorizon.Domain.Interfaces;
using EventHorizon.Domain.ValueObjects;

namespace EventHorizon.Domain.Services;

public class CommissionCalculator : ICommissionCalculator
{
    public CommissionResult Compute(Payment payment, Policy policy)
    {
        var threshold = policy.ThresholdMultiplier * payment.APE;
        
        string methodUsed;
        decimal commissionBase;
        
        if (payment.Receipts <= threshold)
        {
            methodUsed = "APE";
            commissionBase = payment.APE * (policy.ProductRatePct / 100m);
        }
        else
        {
            methodUsed = "Receipts";
            commissionBase = payment.Receipts * (policy.ProductRatePct / 100m);
        }
        
        var poolAmount = commissionBase * (1 - policy.MarginPct / 100m);
        
        var advisorShare = Math.Round(poolAmount * (policy.SplitAdvisor / 100m), 2);
        var introducerShare = Math.Round(poolAmount * (policy.SplitIntroducer / 100m), 2);
        var managerShare = Math.Round(poolAmount * (policy.SplitManager / 100m), 2);
        var execShare = Math.Round(poolAmount * (policy.SplitExec / 100m), 2);
        
        return new CommissionResult(
            methodUsed,
            policy.ProductRatePct,
            policy.MarginPct,
            Math.Round(commissionBase, 2),
            Math.Round(poolAmount, 2),
            advisorShare,
            introducerShare,
            managerShare,
            execShare
        );
    }
}