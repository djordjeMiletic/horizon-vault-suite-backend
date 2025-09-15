using AutoMapper;
using EventHorizon.Application.Common;
using EventHorizon.Application.DTOs;
using EventHorizon.Application.Interfaces;
using EventHorizon.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EventHorizon.Application.Services;

public class PoliciesService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public PoliciesService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<PolicyDto>> GetAllAsync()
    {
        var policies = await _unitOfWork.Repository<Policy>().GetAllAsync();
        return _mapper.Map<IEnumerable<PolicyDto>>(policies);
    }

    public async Task<PolicyDto?> GetByIdAsync(Guid id)
    {
        var policy = await _unitOfWork.Repository<Policy>().GetByIdAsync(id);
        return policy == null ? null : _mapper.Map<PolicyDto>(policy);
    }

    public async Task<PolicyDto> CreateAsync(PolicyCreateDto dto)
    {
        var policy = _mapper.Map<Policy>(dto);
        policy.Id = Guid.NewGuid();
        
        await _unitOfWork.Repository<Policy>().AddAsync(policy);
        await _unitOfWork.SaveChangesAsync();
        
        return _mapper.Map<PolicyDto>(policy);
    }

    public async Task<PolicyDto?> UpdateAsync(Guid id, PolicyUpdateDto dto)
    {
        var policy = await _unitOfWork.Repository<Policy>().GetByIdAsync(id);
        if (policy == null) return null;

        _mapper.Map(dto, policy);
        _unitOfWork.Repository<Policy>().Update(policy);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<PolicyDto>(policy);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var policy = await _unitOfWork.Repository<Policy>().GetByIdAsync(id);
        if (policy == null) return false;

        _unitOfWork.Repository<Policy>().Remove(policy);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}