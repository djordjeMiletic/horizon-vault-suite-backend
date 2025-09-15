using Microsoft.AspNetCore.Mvc;

namespace EventHorizon.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    [HttpPost("demo-login")]
    public IActionResult DemoLogin([FromBody] DemoLoginRequest request)
    {
        // Demo authentication - set headers for subsequent requests
        Response.Headers.Add("X-User-Email", request.Email);
        Response.Headers.Add("X-User-Role", request.Role);
        
        return Ok(new { 
            email = request.Email, 
            role = request.Role,
            message = "Demo login successful. Use X-User-Email and X-User-Role headers for subsequent requests." 
        });
    }

    [HttpGet("session")]
    public IActionResult GetSession()
    {
        var email = Request.Headers["X-User-Email"].FirstOrDefault();
        var role = Request.Headers["X-User-Role"].FirstOrDefault();

        if (string.IsNullOrEmpty(email))
            return Unauthorized();

        return Ok(new { email, role });
    }
}

public class DemoLoginRequest
{
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
}