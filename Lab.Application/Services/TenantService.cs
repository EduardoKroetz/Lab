using AutoMapper;
using Lab.Application.Common.Interfaces;
using Lab.Application.Common.Models;
using Lab.Application.DTOs.Tenants;
using Lab.Domain.Entities;
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

    public async Task<Result<GetTenantResponse>> GetCurrentAsync()
    {
        var tenantId = _tenantProvider.TenantId;

        var tenant = await _dbContext.Tenants
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == tenantId);

        if (tenant == null)
            return Result<GetTenantResponse>.Failure("Tenant não encontrado");

        var response = _mapper.Map<GetTenantResponse>(tenant);

        return Result<GetTenantResponse>.Success(response);
    }

    public async Task<Result<GetTenantResponse>> CreateAsync(InsertTenantRequest request)
    {
        var tenant = new Tenant(request.Name);

        var transaction = await _dbContext.BeginTransactionAsync();

        try
        {
            await _dbContext.Tenants.AddAsync(tenant);
            await _dbContext.SaveChangesAsync();

            var result = await _identityService.CreateUserAsync(
                email: request.User.Email,
                password: request.User.Password,
                tenantId: tenant.Id
            );

            if (!result.Succeeded)
                return Result<GetTenantResponse>.Failure(result.Errors);

            await transaction.CommitAsync();

            var response = _mapper.Map<GetTenantResponse>(tenant);

            return Result<GetTenantResponse>.Success(response);
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<Result<GetTenantResponse>> UpdateCurrentAsync(UpdateCurrentTenantRequest request)
    {
        var tenantId = _tenantProvider.TenantId;

        var tenant = await _dbContext.Tenants.FindAsync(tenantId);
        if (tenant == null)
            return Result<GetTenantResponse>.Failure("Tenant não encontrado");

        tenant.Update(request.Name);

        await _dbContext.SaveChangesAsync();

        var response = _mapper.Map<GetTenantResponse>(tenant);

        return Result<GetTenantResponse>.Success(response);
    }
}
