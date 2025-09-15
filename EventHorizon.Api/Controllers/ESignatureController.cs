using EventHorizon.Application.DTOs;
using EventHorizon.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventHorizon.Api.Controllers;

[ApiController]
[Route("esignature")]
[Authorize]
public class ESignatureController : ControllerBase
{
    private readonly SignatureService _signatureService;

    public ESignatureController(SignatureService signatureService)
    {
        _signatureService = signatureService;
    }

    [HttpPost("requests")]
    public async Task<ActionResult<SignatureRequestDto>> CreateSignatureRequest([FromBody] SignatureCreateRequest request)
    {
        var signatureRequest = await _signatureService.CreateAsync(request);
        return CreatedAtAction(nameof(GetSignatureRequest), new { id = signatureRequest.Id }, signatureRequest);
    }

    [HttpGet("requests/{id}")]
    public async Task<ActionResult<SignatureRequestDto>> GetSignatureRequest(Guid id)
    {
        var request = await _signatureService.GetAsync(id);
        if (request == null) return NotFound();
        return Ok(request);
    }

    [HttpPost("requests/{id}/complete")]
    public async Task<IActionResult> CompleteSignature(Guid id, [FromBody] CompleteSignatureRequest request)
    {
        var success = await _signatureService.CompleteAsync(id, request.Status);
        if (!success) return NotFound();
        return NoContent();
    }

    [HttpGet("/sign/{token}")]
    [AllowAnonymous]
    public async Task<IActionResult> SignDocument(string token)
    {
        var request = await _signatureService.GetByTokenAsync(token);
        if (request == null) return NotFound();

        var html = $@"
<!DOCTYPE html>
<html>
<head>
    <title>Document Signature</title>
    <style>
        body {{ font-family: Arial, sans-serif; max-width: 600px; margin: 50px auto; padding: 20px; }}
        .btn {{ padding: 10px 20px; margin: 10px; border: none; border-radius: 5px; cursor: pointer; }}
        .btn-success {{ background-color: #28a745; color: white; }}
        .btn-danger {{ background-color: #dc3545; color: white; }}
    </style>
</head>
<body>
    <h2>Document Signature Request</h2>
    <p>Please review and sign the document.</p>
    <p><strong>Signer:</strong> {request.SignerEmail}</p>
    <p><strong>Status:</strong> {request.Status}</p>
    
    {(request.Status == "Pending" ? @"
    <button class='btn btn-success' onclick='complete(""Signed"")'>Sign Document</button>
    <button class='btn btn-danger' onclick='complete(""Declined"")'>Decline</button>
    " : $"<p>This document has been {request.Status.ToLower()}.</p>")}
    
    <script>
        function complete(status) {{
            fetch('/esignature/requests/{request.Id}/complete', {{
                method: 'POST',
                headers: {{ 'Content-Type': 'application/json' }},
                body: JSON.stringify({{ status: status }})
            }}).then(() => location.reload());
        }}
    </script>
</body>
</html>";

        return Content(html, "text/html");
    }
}

public class CompleteSignatureRequest
{
    public string Status { get; set; } = string.Empty;
}