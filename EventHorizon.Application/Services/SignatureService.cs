using AutoMapper;
using EventHorizon.Application.DTOs;
using EventHorizon.Application.Interfaces;
using EventHorizon.Domain.Entities;

namespace EventHorizon.Application.Services;

public class SignatureService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public SignatureService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<SignatureRequestDto> CreateAsync(SignatureCreateRequest request)
    {
        var signatureRequest = new SignatureRequest
        {
            Id = Guid.NewGuid(),
            DocumentId = request.DocumentId,
            SignerEmail = request.SignerEmail,
            Status = "Pending",
            CreatedAt = DateTime.UtcNow,
            Token = Guid.NewGuid().ToString("N")
        };

        await _unitOfWork.Repository<SignatureRequest>().AddAsync(signatureRequest);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<SignatureRequestDto>(signatureRequest);
    }

    public async Task<SignatureRequestDto?> GetAsync(Guid id)
    {
        var request = await _unitOfWork.Repository<SignatureRequest>().GetByIdAsync(id);
        return request == null ? null : _mapper.Map<SignatureRequestDto>(request);
    }

    public async Task<SignatureRequestDto?> GetByTokenAsync(string token)
    {
        var request = await _unitOfWork.Repository<SignatureRequest>().Query()
            .FirstOrDefaultAsync(sr => sr.Token == token);
        return request == null ? null : _mapper.Map<SignatureRequestDto>(request);
    }

    public async Task<bool> CompleteAsync(Guid id, string status)
    {
        var request = await _unitOfWork.Repository<SignatureRequest>().GetByIdAsync(id);
        if (request == null) return false;

        request.Status = status;
        request.CompletedAt = DateTime.UtcNow;

        if (status == "Signed")
        {
            var document = await _unitOfWork.Repository<Document>().GetByIdAsync(request.DocumentId);
            if (document != null)
            {
                document.SignedAt = DateTime.UtcNow;
                _unitOfWork.Repository<Document>().Update(document);
            }
        }

        _unitOfWork.Repository<SignatureRequest>().Update(request);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}