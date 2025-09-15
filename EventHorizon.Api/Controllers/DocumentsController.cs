using EventHorizon.Application.Common;
using EventHorizon.Application.DTOs;
using EventHorizon.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EventHorizon.Api.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public class DocumentsController : ControllerBase
{
    private readonly DocumentsService _documentsService;

    public DocumentsController(DocumentsService documentsService)
    {
        _documentsService = documentsService;
    }

    [HttpPost]
    public async Task<ActionResult<DocumentDto>> UploadDocument([FromForm] DocumentUploadRequest request)
    {
        var currentUserEmail = User.FindFirst(ClaimTypes.Email)?.Value;
        var currentUserRole = User.FindFirst(ClaimTypes.Role)?.Value;

        // Set owner email if not provided
        if (string.IsNullOrEmpty(request.OwnerEmail))
            request.OwnerEmail = currentUserEmail;

        // Non-admins can only upload documents for themselves
        if (currentUserRole != "Administrator" && request.OwnerEmail != currentUserEmail)
            return Forbid();

        var document = await _documentsService.UploadAsync(request);
        return CreatedAtAction(nameof(GetDocument), new { id = document.Id }, document);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<DocumentDto>> GetDocument(Guid id)
    {
        var document = await _documentsService.GetAsync(id);
        if (document == null) return NotFound();

        var currentUserEmail = User.FindFirst(ClaimTypes.Email)?.Value;
        var currentUserRole = User.FindFirst(ClaimTypes.Role)?.Value;

        // Check access permissions
        if (currentUserRole != "Administrator" && document.OwnerEmail != currentUserEmail)
            return Forbid();

        return Ok(document);
    }

    [HttpGet]
    public async Task<ActionResult<PaginatedResult<DocumentDto>>> GetDocuments(
        [FromQuery] string? ownerEmail = null,
        [FromQuery] Guid? caseId = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 12)
    {
        var currentUserEmail = User.FindFirst(ClaimTypes.Email)?.Value;
        var currentUserRole = User.FindFirst(ClaimTypes.Role)?.Value;

        // Non-admins can only see their own documents
        if (currentUserRole != "Administrator")
            ownerEmail = currentUserEmail;

        var documents = await _documentsService.ListAsync(ownerEmail, caseId, page, pageSize);
        return Ok(documents);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteDocument(Guid id)
    {
        var document = await _documentsService.GetAsync(id);
        if (document == null) return NotFound();

        var currentUserEmail = User.FindFirst(ClaimTypes.Email)?.Value;
        var currentUserRole = User.FindFirst(ClaimTypes.Role)?.Value;

        // Check delete permissions
        if (currentUserRole != "Administrator" && document.OwnerEmail != currentUserEmail)
            return Forbid();

        var success = await _documentsService.DeleteAsync(id);
        if (!success) return NotFound();
        return NoContent();
    }
}