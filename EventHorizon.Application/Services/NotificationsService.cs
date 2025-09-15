using AutoMapper;
using EventHorizon.Application.DTOs;
using EventHorizon.Application.Interfaces;
using EventHorizon.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EventHorizon.Application.Services;

public class NotificationsService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public NotificationsService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<NotificationDto>> ListAsync(string scope, string? currentUserEmail = null)
    {
        var query = _unitOfWork.Repository<Notification>().Query();

        query = scope switch
        {
            "current" => query.Where(n => n.RecipientEmail == currentUserEmail),
            "admin" => query.Where(n => n.RecipientEmail == null),
            "client" => query.Where(n => n.RecipientEmail == currentUserEmail),
            _ => query.Where(n => n.RecipientEmail == currentUserEmail)
        };

        var notifications = await query
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync();

        return _mapper.Map<IEnumerable<NotificationDto>>(notifications);
    }

    public async Task<bool> MarkReadAsync(Guid id)
    {
        var notification = await _unitOfWork.Repository<Notification>().GetByIdAsync(id);
        if (notification == null) return false;

        notification.Read = true;
        _unitOfWork.Repository<Notification>().Update(notification);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    public async Task MarkAllReadAsync(string? recipientEmail = null, string? scope = null)
    {
        var query = _unitOfWork.Repository<Notification>().Query().Where(n => !n.Read);

        if (!string.IsNullOrEmpty(recipientEmail))
            query = query.Where(n => n.RecipientEmail == recipientEmail);

        if (scope == "admin")
            query = query.Where(n => n.RecipientEmail == null);

        var notifications = await query.ToListAsync();
        foreach (var notification in notifications)
        {
            notification.Read = true;
            _unitOfWork.Repository<Notification>().Update(notification);
        }

        await _unitOfWork.SaveChangesAsync();
    }
}