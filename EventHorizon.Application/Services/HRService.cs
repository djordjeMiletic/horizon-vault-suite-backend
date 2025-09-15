using AutoMapper;
using EventHorizon.Application.Common;
using EventHorizon.Application.DTOs;
using EventHorizon.Application.Interfaces;
using EventHorizon.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EventHorizon.Application.Services;

public class HRService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public HRService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    // Jobs (Internal HR)
    public async Task<PaginatedResult<JobPostingDto>> GetJobsAsync(int page = 1, int pageSize = 12)
    {
        var query = _unitOfWork.Repository<JobPosting>().Query();
        
        var totalCount = await query.CountAsync();
        var items = await query
            .OrderByDescending(j => j.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PaginatedResult<JobPostingDto>
        {
            Items = _mapper.Map<IEnumerable<JobPostingDto>>(items),
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount
        };
    }

    // Applications
    public async Task<PaginatedResult<JobApplicationDto>> GetApplicationsAsync(
        string? status = null, 
        Guid? jobId = null, 
        int page = 1, 
        int pageSize = 12)
    {
        var query = _unitOfWork.Repository<JobApplication>().Query().Include(a => a.JobPosting);

        if (!string.IsNullOrEmpty(status))
            query = query.Where(a => a.Status == status);

        if (jobId.HasValue)
            query = query.Where(a => a.JobPostingId == jobId.Value);

        var totalCount = await query.CountAsync();
        var items = await query
            .OrderByDescending(a => a.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PaginatedResult<JobApplicationDto>
        {
            Items = _mapper.Map<IEnumerable<JobApplicationDto>>(items),
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount
        };
    }

    public async Task<JobApplicationDto?> GetApplicationAsync(Guid id)
    {
        var application = await _unitOfWork.Repository<JobApplication>().Query()
            .Include(a => a.JobPosting)
            .FirstOrDefaultAsync(a => a.Id == id);

        return application == null ? null : _mapper.Map<JobApplicationDto>(application);
    }

    public async Task<bool> UpdateApplicationStatusAsync(Guid id, string status)
    {
        var application = await _unitOfWork.Repository<JobApplication>().GetByIdAsync(id);
        if (application == null) return false;

        application.Status = status;
        _unitOfWork.Repository<JobApplication>().Update(application);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    // Interviews
    public async Task<IEnumerable<InterviewDto>> GetInterviewsAsync(string? candidateEmail = null, Guid? jobId = null)
    {
        var query = _unitOfWork.Repository<Interview>().Query().Include(i => i.JobApplication);

        if (!string.IsNullOrEmpty(candidateEmail))
            query = query.Where(i => i.JobApplication.ApplicantEmail == candidateEmail);

        if (jobId.HasValue)
            query = query.Where(i => i.JobApplication.JobPostingId == jobId.Value);

        var interviews = await query.OrderBy(i => i.ScheduledAt).ToListAsync();
        return _mapper.Map<IEnumerable<InterviewDto>>(interviews);
    }

    public async Task<InterviewDto> CreateInterviewAsync(InterviewCreateDto dto)
    {
        var interview = _mapper.Map<Interview>(dto);
        interview.Id = Guid.NewGuid();

        await _unitOfWork.Repository<Interview>().AddAsync(interview);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<InterviewDto>(interview);
    }

    public async Task<InterviewDto?> UpdateInterviewAsync(Guid id, InterviewUpdateDto dto)
    {
        var interview = await _unitOfWork.Repository<Interview>().GetByIdAsync(id);
        if (interview == null) return null;

        _mapper.Map(dto, interview);
        _unitOfWork.Repository<Interview>().Update(interview);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<InterviewDto>(interview);
    }

    public async Task<bool> UpdateInterviewStatusAsync(Guid id, string status)
    {
        var interview = await _unitOfWork.Repository<Interview>().GetByIdAsync(id);
        if (interview == null) return false;

        interview.Status = status;
        _unitOfWork.Repository<Interview>().Update(interview);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteInterviewAsync(Guid id)
    {
        var interview = await _unitOfWork.Repository<Interview>().GetByIdAsync(id);
        if (interview == null) return false;

        _unitOfWork.Repository<Interview>().Remove(interview);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    // Onboarding Tasks
    public async Task<IEnumerable<OnboardingTaskDto>> GetOnboardingTasksAsync(string? assigneeEmail = null)
    {
        var query = _unitOfWork.Repository<OnboardingTask>().Query();

        if (!string.IsNullOrEmpty(assigneeEmail))
            query = query.Where(t => t.AssigneeEmail == assigneeEmail);

        var tasks = await query.OrderBy(t => t.CreatedAt).ToListAsync();
        return _mapper.Map<IEnumerable<OnboardingTaskDto>>(tasks);
    }

    public async Task<OnboardingTaskDto> CreateOnboardingTaskAsync(OnboardingTaskCreateDto dto)
    {
        var task = _mapper.Map<OnboardingTask>(dto);
        task.Id = Guid.NewGuid();

        await _unitOfWork.Repository<OnboardingTask>().AddAsync(task);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<OnboardingTaskDto>(task);
    }

    public async Task<OnboardingTaskDto?> UpdateOnboardingTaskAsync(Guid id, OnboardingTaskUpdateDto dto)
    {
        var task = await _unitOfWork.Repository<OnboardingTask>().GetByIdAsync(id);
        if (task == null) return null;

        _mapper.Map(dto, task);
        _unitOfWork.Repository<OnboardingTask>().Update(task);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<OnboardingTaskDto>(task);
    }

    public async Task<bool> CompleteOnboardingTaskAsync(Guid id)
    {
        var task = await _unitOfWork.Repository<OnboardingTask>().GetByIdAsync(id);
        if (task == null) return false;

        task.Status = "Completed";
        task.CompletedAt = DateTime.UtcNow;
        _unitOfWork.Repository<OnboardingTask>().Update(task);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteOnboardingTaskAsync(Guid id)
    {
        var task = await _unitOfWork.Repository<OnboardingTask>().GetByIdAsync(id);
        if (task == null) return false;

        _unitOfWork.Repository<OnboardingTask>().Remove(task);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}