namespace EventHorizon.Domain.Entities;

public class GoalHistory
{
    public Guid Id { get; set; }
    public Guid GoalId { get; set; }
    public string Month { get; set; } = string.Empty;
    public decimal Achieved { get; set; }

    public Goal Goal { get; set; } = null!;
}