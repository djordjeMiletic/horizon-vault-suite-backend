using AutoMapper;
using EventHorizon.Application.Common;
using EventHorizon.Application.DTOs;
using EventHorizon.Application.Interfaces;
using EventHorizon.Domain.Entities;
using EventHorizon.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EventHorizon.Application.Services;

public class PaymentsService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ICommissionCalculator _commissionCalculator;

    public PaymentsService(IUnitOfWork unitOfWork, IMapper mapper, ICommissionCalculator commissionCalculator)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _commissionCalculator = commissionCalculator;
    }

    public async Task<PaymentWithCommissionDto> AddPaymentAsync(PaymentCreateDto dto)
    {
        var payment = _mapper.Map<Payment>(dto);
        payment.Id = Guid.NewGuid();

        var policy = await _unitOfWork.Repository<Policy>().GetByIdAsync(dto.ProductId);
        if (policy == null)
            throw new ArgumentException("Invalid ProductId");

        await _unitOfWork.Repository<Payment>().AddAsync(payment);
        await _unitOfWork.SaveChangesAsync();

        // Load the payment with product for mapping
        payment.Product = policy;
        var paymentDto = _mapper.Map<PaymentDto>(payment);
        var commission = _commissionCalculator.Compute(payment, policy);

        return new PaymentWithCommissionDto
        {
            Payment = paymentDto,
            Commission = commission
        };
    }

    public async Task<PaginatedResult<PaymentDto>> GetPaymentsAsync(
        DateOnly? from = null,
        DateOnly? to = null,
        string? advisorEmail = null,
        int page = 1,
        int pageSize = 12)
    {
        var query = _unitOfWork.Repository<Payment>().Query().Include(p => p.Product);

        if (from.HasValue)
            query = query.Where(p => p.Date >= from.Value);

        if (to.HasValue)
            query = query.Where(p => p.Date <= to.Value);

        if (!string.IsNullOrEmpty(advisorEmail))
            query = query.Where(p => p.AdvisorEmail == advisorEmail);

        var totalCount = await query.CountAsync();
        var items = await query
            .OrderByDescending(p => p.Date)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PaginatedResult<PaymentDto>
        {
            Items = _mapper.Map<IEnumerable<PaymentDto>>(items),
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount
        };
    }

    public async Task<PaginatedResult<CommissionDetailsRowDto>> GetCommissionDetailsAsync(
        DateOnly? from = null,
        DateOnly? to = null,
        string? advisorEmail = null,
        int page = 1,
        int pageSize = 12)
    {
        var query = _unitOfWork.Repository<Payment>().Query().Include(p => p.Product);

        if (from.HasValue)
            query = query.Where(p => p.Date >= from.Value);

        if (to.HasValue)
            query = query.Where(p => p.Date <= to.Value);

        if (!string.IsNullOrEmpty(advisorEmail))
            query = query.Where(p => p.AdvisorEmail == advisorEmail);

        var totalCount = await query.CountAsync();
        var payments = await query
            .OrderByDescending(p => p.Date)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var details = payments.Select(p =>
        {
            var commission = _commissionCalculator.Compute(p, p.Product);
            return new CommissionDetailsRowDto
            {
                Date = p.Date,
                Provider = p.Provider,
                Product = p.Product.ProductName,
                MethodUsed = commission.MethodUsed,
                ProductRatePct = commission.ProductRatePct,
                MarginPct = commission.MarginPct,
                CommissionBase = commission.CommissionBase,
                PoolAmount = commission.PoolAmount,
                AdvisorShare = commission.AdvisorShare,
                IntroducerShare = commission.IntroducerShare,
                ManagerShare = commission.ManagerShare,
                ExecShare = commission.ExecSalesManagerShare
            };
        });

        return new PaginatedResult<CommissionDetailsRowDto>
        {
            Items = details,
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount
        };
    }
}