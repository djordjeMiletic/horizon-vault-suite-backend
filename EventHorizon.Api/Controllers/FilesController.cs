using EventHorizon.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EventHorizon.Api.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public class FilesController : ControllerBase
{
    private readonly DocumentsService _documentsService;

    public FilesController(DocumentsService documentsService)
    {
        _documentsService = documentsService;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetFile(Guid id)
    {
        var document = await _documentsService.GetAsync(id);
        if (document == null) return NotFound();

        var currentUserEmail = User.FindFirst(ClaimTypes.Email)?.Value;
        var currentUserRole = User.FindFirst(ClaimTypes.Role)?.Value;

        // Check access permissions
        if (currentUserRole != "Administrator" && document.OwnerEmail != currentUserEmail)
            return Forbid();

        var (stream, contentType, fileName) = await _documentsService.GetFileStreamAsync(id);
        if (stream == null) return NotFound();

        return File(stream, contentType ?? "application/octet-stream", fileName);
    }
}