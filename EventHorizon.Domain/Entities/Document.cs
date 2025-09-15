namespace EventHorizon.Domain.Entities;

public class Document
{
    public Guid Id { get; set; }
    public string OwnerEmail { get; set; } = string.Empty;
    public Guid? CaseId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string OriginalName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long SizeBytes { get; set; }
    public string StoragePath { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? SignedAt { get; set; }
    public string? Tags { get; set; }

    public ICollection<SignatureRequest> SignatureRequests { get; set; } = new List<SignatureRequest>();
}