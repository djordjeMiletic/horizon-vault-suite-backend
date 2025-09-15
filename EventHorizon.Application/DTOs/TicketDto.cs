namespace EventHorizon.Application.DTOs;

public class TicketDto
{
    public Guid Id { get; set; }
    public string Subject { get; set; } = string.Empty;
    public string FromEmail { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string Priority { get; set; } = string.Empty;
    public DateTime UpdatedAt { get; set; }
    public List<TicketMessageDto> Messages { get; set; } = new();
}

public class TicketCreateDto
{
    public string Subject { get; set; } = string.Empty;
    public string Priority { get; set; } = "Medium";
    public string InitialMessage { get; set; } = string.Empty;
}

public class TicketMessageDto
{
    public Guid Id { get; set; }
    public DateTime At { get; set; }
    public string FromEmail { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
}

public class TicketReplyDto
{
    public string Text { get; set; } = string.Empty;
}