using EventHorizon.Domain.Enums;

namespace EventHorizon.Application.DTOs;

public class GoalDto
{
    public Guid Id { get; set; }
    public SubjectType SubjectType { get; set; }
    public string SubjectRef { get; set; } = string.Empty;
    public decimal MonthlyTarget { get; set; }
    public List<GoalHistoryDto> History { get; set; } = new();
}

public class GoalHistoryDto
{
    public Guid Id { get; set; }
    public string Month { get; set; } = string.Empty;
    public decimal Achieved { get; set; }
}