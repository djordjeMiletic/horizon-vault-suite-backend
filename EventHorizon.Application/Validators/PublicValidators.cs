using EventHorizon.Application.DTOs;
using FluentValidation;

namespace EventHorizon.Application.Validators;

public class JobApplicationCreateDtoValidator : AbstractValidator<JobApplicationCreateDto>
{
    public JobApplicationCreateDtoValidator()
    {
        RuleFor(x => x.JobPostingId).NotEmpty();
        RuleFor(x => x.ApplicantName).NotEmpty().MaximumLength(200);
        RuleFor(x => x.ApplicantEmail).NotEmpty().EmailAddress();
        RuleFor(x => x.CoverLetter).MaximumLength(2000);
    }
}

public class WebsiteInquiryCreateDtoValidator : AbstractValidator<WebsiteInquiryCreateDto>
{
    public WebsiteInquiryCreateDtoValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Subject).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Message).NotEmpty().MaximumLength(2000);
        RuleFor(x => x.Source).MaximumLength(100);
    }
}