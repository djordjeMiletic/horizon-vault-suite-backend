using AutoMapper;
using EventHorizon.Application.DTOs;
using EventHorizon.Application.Interfaces;
using EventHorizon.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EventHorizon.Application.Services;

public class PublicFormsService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly string _cvStoragePath;

    public PublicFormsService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _cvStoragePath = Path.Combine(Directory.GetCurrentDirectory(), "storage", "documents", "cv");
        Directory.CreateDirectory(_cvStoragePath);
    }

    public async Task<Guid> CreateJobApplicationAsync(JobApplicationCreateDto dto)
    {
        var application = _mapper.Map<JobApplication>(dto);
        application.Id = Guid.NewGuid();

        if (dto.CV != null)
        {
            var extension = Path.GetExtension(dto.CV.FileName);
            var fileName = $"{application.Id}{extension}";
            var fullPath = Path.Combine(_cvStoragePath, fileName);

            using var stream = new FileStream(fullPath, FileMode.Create);
            await dto.CV.CopyToAsync(stream);

            application.CVPath = Path.Combine("storage", "documents", "cv", fileName);
        }

        await _unitOfWork.Repository<JobApplication>().AddAsync(application);
        await _unitOfWork.SaveChangesAsync();

        return application.Id;
    }

    public async Task<Guid> CreateInquiryAsync(WebsiteInquiryCreateDto dto)
    {
        var inquiry = _mapper.Map<WebsiteInquiry>(dto);
        inquiry.Id = Guid.NewGuid();

        await _unitOfWork.Repository<WebsiteInquiry>().AddAsync(inquiry);
        await _unitOfWork.SaveChangesAsync();

        return inquiry.Id;
    }

    public async Task<IEnumerable<JobPostingDto>> ListOpenJobPostingsAsync()
    {
        var postings = await _unitOfWork.Repository<JobPosting>().Query()
            .Where(jp => jp.IsOpen)
            .OrderByDescending(jp => jp.CreatedAt)
            .ToListAsync();

        return _mapper.Map<IEnumerable<JobPostingDto>>(postings);
    }
}