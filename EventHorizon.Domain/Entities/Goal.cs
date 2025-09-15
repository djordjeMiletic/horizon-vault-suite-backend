using EventHorizon.Domain.Enums;

namespace EventHorizon.Domain.Entities;

public class Goal
{
    public Guid Id { get; set; }
    public SubjectType SubjectType { get; set; }
    public string SubjectRef { get; set; } = string.Empty;
    public decimal MonthlyTarget { get; set; }

    public ICollection<GoalHistory> History { get; set; } = new List<GoalHistory>();
}