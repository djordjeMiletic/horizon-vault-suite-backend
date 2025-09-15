using AutoMapper;
using EventHorizon.Application.DTOs;
using EventHorizon.Domain.Entities;

namespace EventHorizon.Application.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Policy, PolicyDto>();
        CreateMap<PolicyCreateDto, Policy>();
        CreateMap<PolicyUpdateDto, Policy>();

        CreateMap<Payment, PaymentDto>()
            .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.ProductName));
        CreateMap<PaymentCreateDto, Payment>();

        CreateMap<PaymentCycle, PaymentCycleDto>();
        CreateMap<PaymentCycleCreateDto, PaymentCycle>()
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => "Open"));

        CreateMap<PaymentCycleItem, PaymentCycleItemDto>();

        CreateMap<Goal, GoalDto>();
        CreateMap<GoalHistory, GoalHistoryDto>();

        CreateMap<Notification, NotificationDto>();

        CreateMap<Ticket, TicketDto>();
        CreateMap<TicketCreateDto, Ticket>()
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => "Open"));

        CreateMap<TicketMessage, TicketMessageDto>();

        CreateMap<Document, DocumentDto>()
            .ForMember(dest => dest.DownloadUrl, opt => opt.MapFrom(src => $"/files/{src.Id}"));

        CreateMap<SignatureRequest, SignatureRequestDto>()
            .ForMember(dest => dest.SigningUrl, opt => opt.MapFrom(src => $"/sign/{src.Token}"));

        CreateMap<JobPosting, JobPostingDto>();
        CreateMap<JobApplicationCreateDto, JobApplication>()
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => "New"));

        CreateMap<JobApplication, JobApplicationDto>()
            .ForMember(dest => dest.JobTitle, opt => opt.MapFrom(src => src.JobPosting.Title));

        CreateMap<WebsiteInquiryCreateDto, WebsiteInquiry>()
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));

        // HR
        CreateMap<Interview, InterviewDto>();
        CreateMap<InterviewCreateDto, Interview>()
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => "Scheduled"));
        CreateMap<InterviewUpdateDto, Interview>();

        CreateMap<OnboardingTask, OnboardingTaskDto>();
        CreateMap<OnboardingTaskCreateDto, OnboardingTask>()
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => "Pending"));
        CreateMap<OnboardingTaskUpdateDto, OnboardingTask>();

        // Client Management
        CreateMap<Appointment, AppointmentDto>();
        CreateMap<AppointmentCreateDto, Appointment>()
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => "Scheduled"));
        CreateMap<AppointmentUpdateDto, Appointment>();

        // CRM
        CreateMap<Lead, LeadDto>();
        CreateMap<LeadCreateDto, Lead>()
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => "New"));
        CreateMap<LeadUpdateDto, Lead>();

        CreateMap<PipelineDeal, PipelineDealDto>();
        CreateMap<PipelineDealCreateDto, PipelineDeal>()
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));
        CreateMap<PipelineDealUpdateDto, PipelineDeal>()
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));

        CreateMap<ReferralPartner, ReferralPartnerDto>();
        CreateMap<ReferralPartnerCreateDto, ReferralPartner>()
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.Active, opt => opt.MapFrom(src => true));
        CreateMap<ReferralPartnerUpdateDto, ReferralPartner>();

        // Compliance
        CreateMap<ComplianceDoc, ComplianceDocDto>();
        CreateMap<ComplianceDocCreateDto, ComplianceDoc>()
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => "Draft"));
        CreateMap<ComplianceDocUpdateDto, ComplianceDoc>()
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));

        CreateMap<AuditLog, AuditEntryDto>()
            .ForMember(dest => dest.At, opt => opt.MapFrom(src => src.Ts))
            .ForMember(dest => dest.Details, opt => opt.MapFrom(src => src.DetailsJson));
    }
}