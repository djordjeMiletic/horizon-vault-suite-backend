using AutoMapper;
using EventHorizon.Application.DTOs;
using EventHorizon.Application.Interfaces;
using EventHorizon.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EventHorizon.Application.Services;

public class ClientService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public ClientService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    // Appointments
    public async Task<IEnumerable<AppointmentDto>> GetAppointmentsAsync(
        string? ownerEmail = null,
        string? advisorEmail = null,
        DateOnly? from = null,
        DateOnly? to = null)
    {
        var query = _unitOfWork.Repository<Appointment>().Query();

        if (!string.IsNullOrEmpty(ownerEmail))
            query = query.Where(a => a.ClientEmail == ownerEmail);

        if (!string.IsNullOrEmpty(advisorEmail))
            query = query.Where(a => a.AdvisorEmail == advisorEmail);

        if (from.HasValue)
            query = query.Where(a => DateOnly.FromDateTime(a.StartAt) >= from.Value);

        if (to.HasValue)
            query = query.Where(a => DateOnly.FromDateTime(a.StartAt) <= to.Value);

        var appointments = await query.OrderBy(a => a.StartAt).ToListAsync();
        return _mapper.Map<IEnumerable<AppointmentDto>>(appointments);
    }

    public async Task<AppointmentDto> CreateAppointmentAsync(AppointmentCreateDto dto)
    {
        var appointment = _mapper.Map<Appointment>(dto);
        appointment.Id = Guid.NewGuid();

        await _unitOfWork.Repository<Appointment>().AddAsync(appointment);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<AppointmentDto>(appointment);
    }

    public async Task<AppointmentDto?> UpdateAppointmentAsync(Guid id, AppointmentUpdateDto dto)
    {
        var appointment = await _unitOfWork.Repository<Appointment>().GetByIdAsync(id);
        if (appointment == null) return null;

        _mapper.Map(dto, appointment);
        _unitOfWork.Repository<Appointment>().Update(appointment);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<AppointmentDto>(appointment);
    }

    public async Task<bool> DeleteAppointmentAsync(Guid id)
    {
        var appointment = await _unitOfWork.Repository<Appointment>().GetByIdAsync(id);
        if (appointment == null) return false;

        _unitOfWork.Repository<Appointment>().Remove(appointment);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}