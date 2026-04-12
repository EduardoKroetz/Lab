using Lab.Api.Common;
using Lab.Api.Data;
using Lab.Api.DTOs;
using Lab.Api.Entities;
using Lab.Api.Filters;
using Lab.Api.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

var dbConnection = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new ArgumentNullException("Invalid connection string");
builder.Services.AddDbContext<LabDbContext>(x => {
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

builder.Services.Configure<ApiBehaviorOptions>(options => {
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
