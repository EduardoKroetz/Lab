using Lab.Api.Filters;
using Lab.Application.Common;
using Lab.Application.Services;
using Lab.Infrastructure;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = actionContext =>
    {
        var errors = actionContext.ModelState
            .Where(e => e.Value?.Errors.Count > 0)
            .Select(e => e.Value?.Errors?.First().ErrorMessage)
            .Where(message => !string.IsNullOrWhiteSpace(message))
            .ToArray();

        return new BadRequestObjectResult(new ProblemDetails
        {
            Detail = string.Join(" ", errors)
        });
    };
});

builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

builder.AddInfrastructureServices();

builder.Services.AddScoped<AssetService>();
builder.Services.AddScoped<OfferingService>();
builder.Services.AddScoped<ThreatService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<VulnerabilityService>();
builder.Services.AddScoped<CustomerService>();
builder.Services.AddScoped<RiskService>();
builder.Services.AddScoped<ControlService>();
builder.Services.AddScoped<RiskControlService>();
builder.Services.AddScoped<IncidentService>();
builder.Services.AddScoped<IncidentImpactService>();
builder.Services.AddScoped<TenantService>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<AppointmentService>();

builder.Services.AddAutoMapper(cfg =>
{
    cfg.AddMaps(typeof(ApplicationMarker).Assembly);
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseExceptionHandler();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
