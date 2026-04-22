using AutoMapper;
using Lab.Application.Common.Interfaces;
using Lab.Application.DTOs.Tenants;
using Lab.Domain.Common.Models;
using Lab.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Lab.Application.Services;

public class TenantService
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IMapper _mapper;
    private readonly ITenantProvider _tenantProvider;

    public TenantService(IApplicationDbContext dbContext, IMapper mapper, ITenantProvider tenantProvider)
    {
        _dbContext = dbContext;
        _mapper = mapper;
        _tenantProvider = tenantProvider;
    }

    public async Task<Result<GetTenantResponse>> GetCurrentAsync()
    {
        var tenantId = _tenantProvider.TenantId;

        var tenant = await _dbContext.Tenants
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == tenantId);

        if (tenant == null)
            return Result<GetTenantResponse>.Failure("Tenant não encontrado");

        return Result<GetTenantResponse>.Success(_mapper.Map<GetTenantResponse>(tenant));
    }

    public async Task<Result<GetTenantResponse>> CreateAsync(UpsertTenantRequest request)
    {
        var tenant = new Tenant(request.Name);

        await _dbContext.Tenants.AddAsync(tenant);
        await _dbContext.SaveChangesAsync();

        var response = _mapper.Map<GetTenantResponse>(tenant);

        return Result<GetTenantResponse>.Success(response);
    }

    public async Task<Result<GetTenantResponse>> UpdateAsync(Guid tenantId, UpsertTenantRequest request)
    {
        var tenant = await _dbContext.Tenants.FindAsync(tenantId);
        if (tenant == null)
            return Result<GetTenantResponse>.Failure("Tenant não encontrado");

        tenant.Update(request.Name);

        await _dbContext.SaveChangesAsync();

        var response = _mapper.Map<GetTenantResponse>(tenant);

        return Result<GetTenantResponse>.Success(response);
    }
}
