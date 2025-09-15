using EventHorizon.Application.DTOs;
using FluentValidation;

namespace EventHorizon.Application.Validators;

public class TicketCreateDtoValidator : AbstractValidator<TicketCreateDto>
{
    public TicketCreateDtoValidator()
    {
        RuleFor(x => x.Subject).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Priority).Must(p => new[] { "Low", "Medium", "High" }.Contains(p));
        RuleFor(x => x.InitialMessage).NotEmpty().MaximumLength(2000);
    }
}

public class TicketReplyDtoValidator : AbstractValidator<TicketReplyDto>
{
    public TicketReplyDtoValidator()
    {
        RuleFor(x => x.Text).NotEmpty().MaximumLength(2000);
    }
}