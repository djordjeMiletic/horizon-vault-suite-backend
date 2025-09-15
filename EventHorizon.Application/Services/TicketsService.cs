using AutoMapper;
using EventHorizon.Application.DTOs;
using EventHorizon.Application.Interfaces;
using EventHorizon.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EventHorizon.Application.Services;

public class TicketsService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public TicketsService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<TicketDto>> ListAsync(bool mine = false, string? currentUserEmail = null)
    {
        var query = _unitOfWork.Repository<Ticket>().Query().Include(t => t.Messages);

        if (mine && !string.IsNullOrEmpty(currentUserEmail))
            query = query.Where(t => t.FromEmail == currentUserEmail);

        var tickets = await query
            .OrderByDescending(t => t.UpdatedAt)
            .ToListAsync();

        return _mapper.Map<IEnumerable<TicketDto>>(tickets);
    }

    public async Task<TicketDto?> GetAsync(Guid id)
    {
        var ticket = await _unitOfWork.Repository<Ticket>().Query()
            .Include(t => t.Messages)
            .FirstOrDefaultAsync(t => t.Id == id);

        return ticket == null ? null : _mapper.Map<TicketDto>(ticket);
    }

    public async Task<TicketDto> CreateAsync(TicketCreateDto dto, string fromEmail)
    {
        var ticket = _mapper.Map<Ticket>(dto);
        ticket.Id = Guid.NewGuid();
        ticket.FromEmail = fromEmail;

        // Add initial message
        var initialMessage = new TicketMessage
        {
            Id = Guid.NewGuid(),
            TicketId = ticket.Id,
            At = DateTime.UtcNow,
            FromEmail = fromEmail,
            Text = dto.InitialMessage
        };

        await _unitOfWork.Repository<Ticket>().AddAsync(ticket);
        await _unitOfWork.Repository<TicketMessage>().AddAsync(initialMessage);
        await _unitOfWork.SaveChangesAsync();

        ticket.Messages = new List<TicketMessage> { initialMessage };
        return _mapper.Map<TicketDto>(ticket);
    }

    public async Task<bool> ReplyAsync(Guid ticketId, TicketReplyDto dto, string fromEmail)
    {
        var ticket = await _unitOfWork.Repository<Ticket>().GetByIdAsync(ticketId);
        if (ticket == null) return false;

        var message = new TicketMessage
        {
            Id = Guid.NewGuid(),
            TicketId = ticketId,
            At = DateTime.UtcNow,
            FromEmail = fromEmail,
            Text = dto.Text
        };

        ticket.UpdatedAt = DateTime.UtcNow;
        
        await _unitOfWork.Repository<TicketMessage>().AddAsync(message);
        _unitOfWork.Repository<Ticket>().Update(ticket);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }
}