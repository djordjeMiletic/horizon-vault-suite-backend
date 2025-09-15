using AutoMapper;
using EventHorizon.Application.Common;
using EventHorizon.Application.DTOs;
using EventHorizon.Application.Interfaces;
using EventHorizon.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EventHorizon.Application.Services;

public class CRMService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CRMService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    // Leads
    public async Task<PaginatedResult<LeadDto>> GetLeadsAsync(
        string? status = null,
        string? ownerEmail = null,
        int page = 1,
        int pageSize = 12)
    {
        var query = _unitOfWork.Repository<Lead>().Query();

        if (!string.IsNullOrEmpty(status))
            query = query.Where(l => l.Status == status);

        if (!string.IsNullOrEmpty(ownerEmail))
            query = query.Where(l => l.OwnerEmail == ownerEmail);

        var totalCount = await query.CountAsync();
        var items = await query
            .OrderByDescending(l => l.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PaginatedResult<LeadDto>
        {
            Items = _mapper.Map<IEnumerable<LeadDto>>(items),
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount
        };
    }

    public async Task<LeadDto> CreateLeadAsync(LeadCreateDto dto)
    {
        var lead = _mapper.Map<Lead>(dto);
        lead.Id = Guid.NewGuid();

        await _unitOfWork.Repository<Lead>().AddAsync(lead);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<LeadDto>(lead);
    }

    public async Task<LeadDto?> UpdateLeadAsync(Guid id, LeadUpdateDto dto)
    {
        var lead = await _unitOfWork.Repository<Lead>().GetByIdAsync(id);
        if (lead == null) return null;

        _mapper.Map(dto, lead);
        _unitOfWork.Repository<Lead>().Update(lead);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<LeadDto>(lead);
    }

    public async Task<bool> AssignLeadAsync(Guid id, string ownerEmail)
    {
        var lead = await _unitOfWork.Repository<Lead>().GetByIdAsync(id);
        if (lead == null) return false;

        lead.OwnerEmail = ownerEmail;
        _unitOfWork.Repository<Lead>().Update(lead);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UpdateLeadStatusAsync(Guid id, string status)
    {
        var lead = await _unitOfWork.Repository<Lead>().GetByIdAsync(id);
        if (lead == null) return false;

        lead.Status = status;
        _unitOfWork.Repository<Lead>().Update(lead);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteLeadAsync(Guid id)
    {
        var lead = await _unitOfWork.Repository<Lead>().GetByIdAsync(id);
        if (lead == null) return false;

        _unitOfWork.Repository<Lead>().Remove(lead);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    // Pipeline
    public async Task<PaginatedResult<PipelineDealDto>> GetPipelineAsync(
        string? stage = null,
        string? advisorEmail = null,
        int page = 1,
        int pageSize = 12)
    {
        var query = _unitOfWork.Repository<PipelineDeal>().Query();

        if (!string.IsNullOrEmpty(stage))
            query = query.Where(p => p.Stage == stage);

        if (!string.IsNullOrEmpty(advisorEmail))
            query = query.Where(p => p.AdvisorEmail == advisorEmail);

        var totalCount = await query.CountAsync();
        var items = await query
            .OrderByDescending(p => p.UpdatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PaginatedResult<PipelineDealDto>
        {
            Items = _mapper.Map<IEnumerable<PipelineDealDto>>(items),
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount
        };
    }

    public async Task<PipelineDealDto> CreatePipelineDealAsync(PipelineDealCreateDto dto)
    {
        var deal = _mapper.Map<PipelineDeal>(dto);
        deal.Id = Guid.NewGuid();

        await _unitOfWork.Repository<PipelineDeal>().AddAsync(deal);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<PipelineDealDto>(deal);
    }

    public async Task<PipelineDealDto?> UpdatePipelineDealAsync(Guid id, PipelineDealUpdateDto dto)
    {
        var deal = await _unitOfWork.Repository<PipelineDeal>().GetByIdAsync(id);
        if (deal == null) return null;

        _mapper.Map(dto, deal);
        _unitOfWork.Repository<PipelineDeal>().Update(deal);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<PipelineDealDto>(deal);
    }

    public async Task<bool> MovePipelineDealAsync(Guid id, string stage)
    {
        var deal = await _unitOfWork.Repository<PipelineDeal>().GetByIdAsync(id);
        if (deal == null) return false;

        deal.Stage = stage;
        deal.UpdatedAt = DateTime.UtcNow;
        _unitOfWork.Repository<PipelineDeal>().Update(deal);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeletePipelineDealAsync(Guid id)
    {
        var deal = await _unitOfWork.Repository<PipelineDeal>().GetByIdAsync(id);
        if (deal == null) return false;

        _unitOfWork.Repository<PipelineDeal>().Remove(deal);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    // Referral Partners
    public async Task<PaginatedResult<ReferralPartnerDto>> GetReferralPartnersAsync(int page = 1, int pageSize = 12)
    {
        var query = _unitOfWork.Repository<ReferralPartner>().Query();

        var totalCount = await query.CountAsync();
        var items = await query
            .OrderByDescending(r => r.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PaginatedResult<ReferralPartnerDto>
        {
            Items = _mapper.Map<IEnumerable<ReferralPartnerDto>>(items),
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount
        };
    }

    public async Task<ReferralPartnerDto> CreateReferralPartnerAsync(ReferralPartnerCreateDto dto)
    {
        var partner = _mapper.Map<ReferralPartner>(dto);
        partner.Id = Guid.NewGuid();

        await _unitOfWork.Repository<ReferralPartner>().AddAsync(partner);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<ReferralPartnerDto>(partner);
    }

    public async Task<ReferralPartnerDto?> UpdateReferralPartnerAsync(Guid id, ReferralPartnerUpdateDto dto)
    {
        var partner = await _unitOfWork.Repository<ReferralPartner>().GetByIdAsync(id);
        if (partner == null) return null;

        _mapper.Map(dto, partner);
        _unitOfWork.Repository<ReferralPartner>().Update(partner);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<ReferralPartnerDto>(partner);
    }

    public async Task<bool> UpdateReferralPartnerStatusAsync(Guid id, bool active)
    {
        var partner = await _unitOfWork.Repository<ReferralPartner>().GetByIdAsync(id);
        if (partner == null) return false;

        partner.Active = active;
        _unitOfWork.Repository<ReferralPartner>().Update(partner);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteReferralPartnerAsync(Guid id)
    {
        var partner = await _unitOfWork.Repository<ReferralPartner>().GetByIdAsync(id);
        if (partner == null) return false;

        _unitOfWork.Repository<ReferralPartner>().Remove(partner);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}