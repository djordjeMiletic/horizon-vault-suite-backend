using Microsoft.AspNetCore.Http;

namespace EventHorizon.Application.DTOs;

public class DocumentDto
{
    public Guid Id { get; set; }
    public string OwnerEmail { get; set; } = string.Empty;
    public Guid? CaseId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string OriginalName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long SizeBytes { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? SignedAt { get; set; }
    public string? Tags { get; set; }
    public string DownloadUrl { get; set; } = string.Empty;
}

public class DocumentUploadRequest
{
    public string? OwnerEmail { get; set; }
    public Guid? CaseId { get; set; }
    public string? Tags { get; set; }
    public IFormFile File { get; set; } = null!;
}