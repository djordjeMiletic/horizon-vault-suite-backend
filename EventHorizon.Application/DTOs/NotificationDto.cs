namespace EventHorizon.Application.DTOs;

public class NotificationDto
{
    public Guid Id { get; set; }
    public string? RecipientEmail { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public bool Read { get; set; }
}