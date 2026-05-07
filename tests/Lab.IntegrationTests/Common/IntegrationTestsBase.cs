using Lab.Domain.Common;
using Lab.Domain.Entities;
using Lab.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Lab.IntegrationTests.Common;

public class IntegrationTestsBase : IAsyncDisposable
{
    public readonly ApplicationDbContext DbContext;
    public readonly ISystemClock Clock;

    protected IntegrationTestsBase()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlite("DataSource=:memory:")
            .Options;

        var tenant = new Tenant("Test Tenant");

        DbContext = new ApplicationDbContext(options, new FakeTenantProvider(tenant.Id));
        DbContext.Database.OpenConnection();
        DbContext.Database.EnsureCreated();

        DbContext.Tenants.Add(tenant);
        DbContext.SaveChanges();

        Clock = new FakeClock(new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc));
    }

    public async ValueTask DisposeAsync()
    {
        await DbContext.Database.CloseConnectionAsync();
        await DbContext.DisposeAsync();
    }

    protected void ClearTracking() => DbContext.ChangeTracker.Clear();
}
