namespace EventHorizon.Domain.Entities;

public class AuditLog
{
    public Guid Id { get; set; }
    public DateTime Ts { get; set; }
    public string Actor { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public string EntityType { get; set; } = string.Empty;
    public string EntityId { get; set; } = string.Empty;
    public string DetailsJson { get; set; } = string.Empty;
}