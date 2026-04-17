using Lab.Api.Domain.Entities;
using Lab.Api.Infrastructure.Data;

namespace Lab.Api.Application.Services;

public class TenantService
{
    private readonly LabDbContext _dbContext;

    public TenantService(LabDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task CreateAsync(Tenant tenant)
    {
        await _dbContext.AddAsync(tenant);
        await _dbContext.SaveChangesAsync();
    }
}
