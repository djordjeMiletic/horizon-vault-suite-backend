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

        CreateMap<WebsiteInquiryCreateDto, WebsiteInquiry>()
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));
    }
}