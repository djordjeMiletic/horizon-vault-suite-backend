namespace EventHorizon.Application.DTOs;

public class SignatureRequestDto
{
    public Guid Id { get; set; }
    public Guid DocumentId { get; set; }
    public string SignerEmail { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string SigningUrl { get; set; } = string.Empty;
}

public class SignatureCreateRequest
{
    public Guid DocumentId { get; set; }
    public string SignerEmail { get; set; } = string.Empty;
}