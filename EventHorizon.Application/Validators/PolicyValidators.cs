using EventHorizon.Application.DTOs;
using FluentValidation;

namespace EventHorizon.Application.Validators;

public class PolicyCreateDtoValidator : AbstractValidator<PolicyCreateDto>
{
    public PolicyCreateDtoValidator()
    {
        RuleFor(x => x.ProductCode).NotEmpty().MaximumLength(50);
        RuleFor(x => x.ProductName).NotEmpty().MaximumLength(200);
        RuleFor(x => x.ProductRatePct).GreaterThan(0).LessThanOrEqualTo(100);
        RuleFor(x => x.MarginPct).GreaterThanOrEqualTo(0).LessThanOrEqualTo(100);
        RuleFor(x => x.ThresholdMultiplier).GreaterThan(0);
        RuleFor(x => x.SplitAdvisor + x.SplitIntroducer + x.SplitManager + x.SplitExec)
            .Equal(100).WithMessage("Split percentages must total 100%");
    }
}

public class PolicyUpdateDtoValidator : AbstractValidator<PolicyUpdateDto>
{
    public PolicyUpdateDtoValidator()
    {
        RuleFor(x => x.ProductCode).NotEmpty().MaximumLength(50);
        RuleFor(x => x.ProductName).NotEmpty().MaximumLength(200);
        RuleFor(x => x.ProductRatePct).GreaterThan(0).LessThanOrEqualTo(100);
        RuleFor(x => x.MarginPct).GreaterThanOrEqualTo(0).LessThanOrEqualTo(100);
        RuleFor(x => x.ThresholdMultiplier).GreaterThan(0);
        RuleFor(x => x.SplitAdvisor + x.SplitIntroducer + x.SplitManager + x.SplitExec)
            .Equal(100).WithMessage("Split percentages must total 100%");
    }
}