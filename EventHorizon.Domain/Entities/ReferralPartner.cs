namespace EventHorizon.Domain.Entities;

public class ReferralPartner
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public bool Active { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public string? Notes { get; set; }
}