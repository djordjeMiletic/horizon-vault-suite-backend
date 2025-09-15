using AutoMapper;
using EventHorizon.Application.DTOs;
using EventHorizon.Application.Interfaces;
using EventHorizon.Domain.Entities;
using EventHorizon.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace EventHorizon.Application.Services;

public class GoalsService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GoalsService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<GoalDto?> GetGoalsAsync(SubjectType subjectType, string subjectRef)
    {
        var goal = await _unitOfWork.Repository<Goal>().Query()
            .Include(g => g.History)
            .FirstOrDefaultAsync(g => g.SubjectType == subjectType && g.SubjectRef == subjectRef);

        if (goal == null) return null;

        var goalDto = _mapper.Map<GoalDto>(goal);

        // Ensure we have 6 months of history
        var months = GetLast6Months();
        var existingHistory = goal.History.ToDictionary(h => h.Month, h => h.Achieved);

        goalDto.History = months.Select(month => new GoalHistoryDto
        {
            Id = Guid.NewGuid(),
            Month = month,
            Achieved = existingHistory.ContainsKey(month) 
                ? existingHistory[month] 
                : await ComputeAchievedForMonth(subjectType, subjectRef, month)
        }).ToList();

        return goalDto;
    }

    private async Task<decimal> ComputeAchievedForMonth(SubjectType subjectType, string subjectRef, string month)
    {
        var query = _unitOfWork.Repository<Payment>().Query();

        if (subjectType == SubjectType.Advisor)
        {
            query = query.Where(p => p.AdvisorEmail == subjectRef);
        }
        else if (subjectType == SubjectType.Manager && subjectRef == "team")
        {
            // For manager "team", sum all advisors
            // This is a simplified approach - in reality you'd have a proper team structure
        }

        var payments = await query
            .Where(p => p.Date.ToString("yyyy-MM") == month)
            .ToListAsync();

        return payments.Sum(p => p.APE);
    }

    private List<string> GetLast6Months()
    {
        var months = new List<string>();
        var now = DateTime.Now;

        for (int i = 5; i >= 0; i--)
        {
            var month = now.AddMonths(-i);
            months.Add(month.ToString("yyyy-MM"));
        }

        return months;
    }
}