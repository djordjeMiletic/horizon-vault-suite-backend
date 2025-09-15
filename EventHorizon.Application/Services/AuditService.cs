using AutoMapper;
using EventHorizon.Application.Common;
using EventHorizon.Application.DTOs;
using EventHorizon.Application.Interfaces;
using EventHorizon.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EventHorizon.Application.Services;

public class AuditService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public AuditService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PaginatedResult<AuditEntryDto>> GetAuditEntriesAsync(
        string? entityType = null,
        string? entityId = null,
        string? actor = null,
        int page = 1,
        int pageSize = 12)
    {
        var query = _unitOfWork.Repository<AuditLog>().Query();

        if (!string.IsNullOrEmpty(entityType))
            query = query.Where(a => a.EntityType == entityType);

        if (!string.IsNullOrEmpty(entityId))
            query = query.Where(a => a.EntityId == entityId);

        if (!string.IsNullOrEmpty(actor))
            query = query.Where(a => a.Actor == actor);

        var totalCount = await query.CountAsync();
        var items = await query
            .OrderByDescending(a => a.Ts)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PaginatedResult<AuditEntryDto>
        {
            Items = _mapper.Map<IEnumerable<AuditEntryDto>>(items),
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount
        };
    }

    public async Task<AuditEntryDto?> GetAuditEntryAsync(Guid id)
    {
        var entry = await _unitOfWork.Repository<AuditLog>().GetByIdAsync(id);
        return entry == null ? null : _mapper.Map<AuditEntryDto>(entry);
    }

    public async Task AppendAuditEntryAsync(string actor, string action, string entityType, string entityId, string? details = null)
    {
        var entry = new AuditLog
        {
            Id = Guid.NewGuid(),
            Ts = DateTime.UtcNow,
            Actor = actor,
            Action = action,
            EntityType = entityType,
            EntityId = entityId,
            DetailsJson = details ?? "{}"
        };

        await _unitOfWork.Repository<AuditLog>().AddAsync(entry);
        await _unitOfWork.SaveChangesAsync();
    }
}