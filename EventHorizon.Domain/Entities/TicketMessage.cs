namespace EventHorizon.Domain.Entities;

public class TicketMessage
{
    public Guid Id { get; set; }
    public Guid TicketId { get; set; }
    public DateTime At { get; set; }
    public string FromEmail { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;

    public Ticket Ticket { get; set; } = null!;
}