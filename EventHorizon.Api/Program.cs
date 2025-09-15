using EventHorizon.Application.Interfaces;
using EventHorizon.Application.Mapping;
using EventHorizon.Application.Services;
using EventHorizon.Application.Validators;
using EventHorizon.Domain.Interfaces;
using EventHorizon.Domain.Services;
using EventHorizon.Infrastructure.Data;
using EventHorizon.Infrastructure.Repositories;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Database
builder.Services.AddDbContext<EventHorizonDbContext>(options =>
    options.UseSqlite("Data Source=./event_horizon.db"));

// Repositories
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

// Domain Services
builder.Services.AddScoped<ICommissionCalculator, CommissionCalculator>();

// Application Services
builder.Services.AddScoped<PoliciesService>();
builder.Services.AddScoped<PaymentsService>();
builder.Services.AddScoped<AnalyticsService>();
builder.Services.AddScoped<GoalsService>();
builder.Services.AddScoped<NotificationsService>();
builder.Services.AddScoped<TicketsService>();
builder.Services.AddScoped<DocumentsService>();
builder.Services.AddScoped<SignatureService>();
builder.Services.AddScoped<PublicFormsService>();
builder.Services.AddScoped<HRService>();
builder.Services.AddScoped<ClientService>();
builder.Services.AddScoped<CRMService>();
builder.Services.AddScoped<ComplianceService>();
builder.Services.AddScoped<AuditService>();

// AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile));

// FluentValidation
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<PolicyCreateDtoValidator>();

// CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Authentication (Demo)
builder.Services.AddAuthentication("Demo")
    .AddScheme<Microsoft.AspNetCore.Authentication.AuthenticationSchemeOptions, DemoAuthenticationHandler>("Demo", options => { });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Manager", policy => policy.RequireRole("Manager", "Administrator"));
    options.AddPolicy("Administrator", policy => policy.RequireRole("Administrator"));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    
    // Seed database
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<EventHorizonDbContext>();
    await context.Database.EnsureCreatedAsync();
    await DataSeeder.SeedAsync(context);
}

app.UseCors();
app.UseAuthentication();
app.UseAuthorization();

// Static files for document serving
app.UseStaticFiles();

app.MapControllers();

// Health endpoint
app.MapGet("/health", () => Results.Ok(new { status = "healthy", timestamp = DateTime.UtcNow }));

app.Run();

// Demo Authentication Handler
public class DemoAuthenticationHandler : Microsoft.AspNetCore.Authentication.AuthenticationHandler<Microsoft.AspNetCore.Authentication.AuthenticationSchemeOptions>
{
    public DemoAuthenticationHandler(Microsoft.Extensions.Options.IOptionsMonitor<Microsoft.AspNetCore.Authentication.AuthenticationSchemeOptions> options,
        Microsoft.Extensions.Logging.ILoggerFactory logger, System.Text.Encodings.Web.UrlEncoder encoder, Microsoft.AspNetCore.Authentication.ISystemClock clock)
        : base(options, logger, encoder, clock) { }

    protected override Task<Microsoft.AspNetCore.Authentication.AuthenticateResult> HandleAuthenticateAsync()
    {
        // Demo authentication - in production, implement proper JWT validation
        var email = Request.Headers["X-User-Email"].FirstOrDefault();
        var role = Request.Headers["X-User-Role"].FirstOrDefault();

        if (string.IsNullOrEmpty(email))
            return Task.FromResult(Microsoft.AspNetCore.Authentication.AuthenticateResult.NoResult());

        var claims = new[]
        {
            new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Email, email),
            new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Role, role ?? "Client")
        };

        var identity = new System.Security.Claims.ClaimsIdentity(claims, "Demo");
        var principal = new System.Security.Claims.ClaimsPrincipal(identity);
        var ticket = new Microsoft.AspNetCore.Authentication.AuthenticationTicket(principal, "Demo");

        return Task.FromResult(Microsoft.AspNetCore.Authentication.AuthenticateResult.Success(ticket));
    }
}