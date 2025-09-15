using AutoMapper;
using EventHorizon.Application.Common;
using EventHorizon.Application.DTOs;
using EventHorizon.Application.Interfaces;
using EventHorizon.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace EventHorizon.Application.Services;

public class DocumentsService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly string _storagePath;

    public DocumentsService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _storagePath = Path.Combine(Directory.GetCurrentDirectory(), "storage", "documents");
        Directory.CreateDirectory(_storagePath);
    }

    public async Task<DocumentDto> UploadAsync(DocumentUploadRequest request)
    {
        var document = new Document
        {
            Id = Guid.NewGuid(),
            OwnerEmail = request.OwnerEmail ?? "system",
            CaseId = request.CaseId,
            OriginalName = request.File.FileName,
            ContentType = request.File.ContentType,
            SizeBytes = request.File.Length,
            Tags = request.Tags,
            CreatedAt = DateTime.UtcNow
        };

        // Generate unique filename
        var extension = Path.GetExtension(request.File.FileName);
        document.FileName = $"{document.Id}{extension}";
        document.StoragePath = Path.Combine("storage", "documents", document.FileName);

        // Save file to disk
        var fullPath = Path.Combine(_storagePath, document.FileName);
        using var stream = new FileStream(fullPath, FileMode.Create);
        await request.File.CopyToAsync(stream);

        await _unitOfWork.Repository<Document>().AddAsync(document);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<DocumentDto>(document);
    }

    public async Task<DocumentDto?> GetAsync(Guid id)
    {
        var document = await _unitOfWork.Repository<Document>().GetByIdAsync(id);
        return document == null ? null : _mapper.Map<DocumentDto>(document);
    }

    public async Task<PaginatedResult<DocumentDto>> ListAsync(
        string? ownerEmail = null,
        Guid? caseId = null,
        int page = 1,
        int pageSize = 12)
    {
        var query = _unitOfWork.Repository<Document>().Query();

        if (!string.IsNullOrEmpty(ownerEmail))
            query = query.Where(d => d.OwnerEmail == ownerEmail);

        if (caseId.HasValue)
            query = query.Where(d => d.CaseId == caseId.Value);

        var totalCount = await query.CountAsync();
        var items = await query
            .OrderByDescending(d => d.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PaginatedResult<DocumentDto>
        {
            Items = _mapper.Map<IEnumerable<DocumentDto>>(items),
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount
        };
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var document = await _unitOfWork.Repository<Document>().GetByIdAsync(id);
        if (document == null) return false;

        // Delete file from disk
        var fullPath = Path.Combine(_storagePath, document.FileName);
        if (File.Exists(fullPath))
            File.Delete(fullPath);

        _unitOfWork.Repository<Document>().Remove(document);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    public async Task<(Stream? stream, string? contentType, string? fileName)> GetFileStreamAsync(Guid id)
    {
        var document = await _unitOfWork.Repository<Document>().GetByIdAsync(id);
        if (document == null) return (null, null, null);

        var fullPath = Path.Combine(_storagePath, document.FileName);
        if (!File.Exists(fullPath)) return (null, null, null);

        var stream = new FileStream(fullPath, FileMode.Open, FileAccess.Read);
        return (stream, document.ContentType, document.OriginalName);
    }
}