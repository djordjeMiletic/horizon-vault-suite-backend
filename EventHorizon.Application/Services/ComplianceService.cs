using AutoMapper;
using EventHorizon.Application.Common;
using EventHorizon.Application.DTOs;
using EventHorizon.Application.Interfaces;
using EventHorizon.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EventHorizon.Application.Services;

public class ComplianceService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public ComplianceService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PaginatedResult<ComplianceDocDto>> GetComplianceDocsAsync(
        string? status = null,
        int page = 1,
        int pageSize = 12)
    {
        var query = _unitOfWork.Repository<ComplianceDoc>().Query();

        if (!string.IsNullOrEmpty(status))
            query = query.Where(c => c.Status == status);

        var totalCount = await query.CountAsync();
        var items = await query
            .OrderByDescending(c => c.UpdatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PaginatedResult<ComplianceDocDto>
        {
            Items = _mapper.Map<IEnumerable<ComplianceDocDto>>(items),
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount
        };
    }

    public async Task<ComplianceDocDto> CreateComplianceDocAsync(ComplianceDocCreateDto dto, string ownerEmail)
    {
        var doc = _mapper.Map<ComplianceDoc>(dto);
        doc.Id = Guid.NewGuid();
        doc.OwnerEmail = ownerEmail;

        await _unitOfWork.Repository<ComplianceDoc>().AddAsync(doc);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<ComplianceDocDto>(doc);
    }

    public async Task<ComplianceDocDto?> UpdateComplianceDocAsync(Guid id, ComplianceDocUpdateDto dto)
    {
        var doc = await _unitOfWork.Repository<ComplianceDoc>().GetByIdAsync(id);
        if (doc == null) return null;

        _mapper.Map(dto, doc);
        _unitOfWork.Repository<ComplianceDoc>().Update(doc);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<ComplianceDocDto>(doc);
    }

    public async Task<bool> DeleteComplianceDocAsync(Guid id)
    {
        var doc = await _unitOfWork.Repository<ComplianceDoc>().GetByIdAsync(id);
        if (doc == null) return false;

        _unitOfWork.Repository<ComplianceDoc>().Remove(doc);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}