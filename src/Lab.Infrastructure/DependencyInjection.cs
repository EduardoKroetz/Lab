using Lab.Application.Common.Interfaces;
using Lab.Domain.Common;
using Lab.Infrastructure.Data;
using Lab.Infrastructure.Identity;
using Lab.Infrastructure.Services;
using Lab.Infrastructure.Time;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Lab.Infrastructure;

public static class DependencyInjection
{
    public static void AddInfrastructureServices(this IHostApplicationBuilder builder)
    {
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddScoped<ITenantProvider, HttpTenantProvider>();

        var dbConnection = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new ArgumentNullException("Invalid connection string");
        builder.Services.AddDbContext<ApplicationDbContext>(x =>
        {
            x.UseSqlServer(dbConnection);
        });

        builder.Services.AddIdentity<ApplicationUser, IdentityRole<Guid>>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders()
            .AddErrorDescriber<PtBrIdentityErrorDescriber>();

        builder.Services.Configure<IdentityOptions>(opt =>
        {
            opt.Password.RequireUppercase = false;
            opt.Password.RequireLowercase = false;
            opt.Password.RequiredLength = 6;
        });

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

        builder.Services.AddSingleton<ISystemClock, SystemClock>();

        builder.Services.AddScoped<IApplicationDbContext>(sp => sp.GetRequiredService<ApplicationDbContext>());

        builder.Services.AddScoped<IUserProvider, HttpUserProvider>();
        builder.Services.AddScoped<IIdentityService, IdentityService>();
        builder.Services.AddScoped<ITokenService, TokenService>();

        builder.Services.AddAuthorization();
    }

}
