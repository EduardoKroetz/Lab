using AutoMapper;
using Lab.Api.Application.DTOs.Tenants;
using Lab.Api.Domain.Entities;
using Lab.Api.Domain.Exceptions;
using Lab.Api.Infrastructure.Data;

namespace Lab.Api.Application.Services;

public class TenantService
{
    private readonly LabDbContext _dbContext;
    private readonly IMapper _mapper;

    public TenantService(LabDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<GetTenantDto> GetCurrentTenantAsync(Guid tenantId)
    {
        var tenant = await _dbContext.Tenants.FindAsync(tenantId);
        if (tenant is null)
            throw new NotFoundException("Tenant não encontrado");

        return _mapper.Map<GetTenantDto>(tenant);
    }

    public async Task<GetTenantDto> CreateAsync(UpsertTenantDto dto)
    {
        var tenant = new Tenant(dto.Name);

        await _dbContext.Tenants.AddAsync(tenant);
        await _dbContext.SaveChangesAsync();

        return _mapper.Map<GetTenantDto>(tenant);
    }

    public async Task<GetTenantDto> UpdateAsync(Guid tenantId, UpsertTenantDto dto)
    {
        var tenant = await _dbContext.Tenants.FindAsync(tenantId);
        if (tenant is null)
            throw new NotFoundException("Tenant não encontrado");

        tenant.Update(dto.Name);

        await _dbContext.SaveChangesAsync();

        return _mapper.Map<GetTenantDto>(tenant);
    }
}
