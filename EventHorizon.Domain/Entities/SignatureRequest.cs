namespace EventHorizon.Domain.Entities;

public class SignatureRequest
{
    public Guid Id { get; set; }
    public Guid DocumentId { get; set; }
    public string SignerEmail { get; set; } = string.Empty;
    public string Status { get; set; } = "Pending";
    public DateTime CreatedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string Token { get; set; } = string.Empty;

    public Document Document { get; set; } = null!;
}