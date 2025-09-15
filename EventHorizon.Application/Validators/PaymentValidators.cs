using EventHorizon.Application.DTOs;
using FluentValidation;

namespace EventHorizon.Application.Validators;

public class PaymentCreateDtoValidator : AbstractValidator<PaymentCreateDto>
{
    public PaymentCreateDtoValidator()
    {
        RuleFor(x => x.Date).NotEmpty();
        RuleFor(x => x.ProductId).NotEmpty();
        RuleFor(x => x.Provider).NotEmpty().MaximumLength(200);
        RuleFor(x => x.APE).GreaterThan(0);
        RuleFor(x => x.Receipts).GreaterThanOrEqualTo(0);
        RuleFor(x => x.AdvisorEmail).NotEmpty().EmailAddress();
        RuleFor(x => x.Notes).MaximumLength(1000);
    }
}