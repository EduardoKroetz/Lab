using Lab.Api.Application.DTOs;
using Lab.Api.Application.Services;
using Lab.Api.Common.Configuration;
using Lab.Api.Common.Filters;
using Lab.Api.Domain.Entities;
using Lab.Api.Infrastructure.Data;
using Lab.Api.Infrastructure.Services;
using Lab.Api.Infrastructure.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ITenantProvider, HttpTenantProvider>();

var dbConnection = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new ArgumentNullException("Invalid connection string");
builder.Services.AddDbContext<LabDbContext>(x =>
{
    x.UseSqlServer(dbConnection);
});

builder.Services.AddIdentity<ApplicationUser, IdentityRole<Guid>>()
    .AddEntityFrameworkStores<LabDbContext>()
    .AddDefaultTokenProviders()
    .AddErrorDescriber<PtBrIdentityErrorDescriber>();

var key = builder.Configuration["Jwt:Key"] ?? throw new ArgumentNullException("Invalid JWT key");

builder.Services.AddAuthentication(opt =>
{
    opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(opt =>
{
    opt.TokenValidationParameters = new()
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
        ValidateIssuerSigningKey = true,
    };
});

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = actionContext =>
    {
        var errors = actionContext.ModelState
            .Where(e => e.Value?.Errors.Count > 0)
            .Select(e => e.Value?.Errors?.First().ErrorMessage);

        var responseDto = new ResponseDto(errors);

        return new BadRequestObjectResult(responseDto);
    };
});

builder.Services.AddAuthorization();

builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

builder.Services.AddScoped<TokenService>();
builder.Services.AddScoped<OfferingService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<CustomerService>();
builder.Services.AddScoped<TenantService>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<AppointmentService>();

builder.Services.AddAutoMapper(cfg =>
{
    cfg.AddMaps(typeof(Program).Assembly);
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
