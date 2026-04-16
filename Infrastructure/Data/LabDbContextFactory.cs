using Lab.Api.Infrastructure.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Lab.Api.Infrastructure.Data;

public sealed class LabDbContextFactory : IDesignTimeDbContextFactory<LabDbContext>
{
    public LabDbContext CreateDbContext(string[] args)
    {
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";

        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false)
            .AddJsonFile($"appsettings.{environment}.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("ConnectionString 'DefaultConnection' not found.");

        var options = new DbContextOptionsBuilder<LabDbContext>()
            .UseSqlServer(connectionString)
            .Options;

        ITenantProvider tenantProvider = new DesignTimeTenantProvider();

        return new LabDbContext(options, tenantProvider);
    }
}

public sealed class DesignTimeTenantProvider : ITenantProvider
{
    public Guid TenantId => Guid.NewGuid();
}