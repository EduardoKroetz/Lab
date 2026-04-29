using AutoMapper;
using Lab.Application.Common.Interfaces;
using Lab.Application.DTOs.Tenants;
using Lab.Domain.Entities;
using Lab.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Lab.Application.Services;

public class TenantService
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IMapper _mapper;
    private readonly ITenantProvider _tenantProvider;
    private readonly IIdentityService _identityService;

    public TenantService(IApplicationDbContext dbContext, IMapper mapper, ITenantProvider tenantProvider, IIdentityService identityService)
    {
        _dbContext = dbContext;
        _mapper = mapper;
        _tenantProvider = tenantProvider;
        _identityService = identityService;
    }

    public async Task<GetTenantResponse> GetCurrentAsync()
    {
        var tenantId = _tenantProvider.TenantId;

        var tenant = await _dbContext.Tenants
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == tenantId);

        if (tenant == null)
            throw new NotFoundException("Tenant não encontrado");

        return _mapper.Map<GetTenantResponse>(tenant);
    }

    public async Task<GetTenantResponse> CreateAsync(InsertTenantRequest request)
    {
        var tenant = new Tenant(request.Name);
        var transaction = await _dbContext.BeginTransactionAsync();

        try
        {
            await _dbContext.Tenants.AddAsync(tenant);
            await _dbContext.SaveChangesAsync();

            await _identityService.CreateUserAsync(
                email: request.User.Email,
                password: request.User.Password,
                tenantId: tenant.Id
            );

            await transaction.CommitAsync();

            return _mapper.Map<GetTenantResponse>(tenant);
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<GetTenantResponse> UpdateCurrentAsync(UpdateCurrentTenantRequest request)
    {
        var tenantId = _tenantProvider.TenantId;

        var tenant = await _dbContext.Tenants.FindAsync(tenantId);
        if (tenant == null)
            throw new NotFoundException("Tenant não encontrado");

        tenant.Update(request.Name);

        await _dbContext.SaveChangesAsync();

        return _mapper.Map<GetTenantResponse>(tenant);
    }
}
