using Lab.Api.Extensions;
using Lab.Application.DTOs.Tenants;
using Lab.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Lab.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TenantsController : ControllerBase
{
    private readonly TenantService _tenantService;

    public TenantsController(TenantService tenantService)
    {
        _tenantService = tenantService;
    }

    [HttpGet("current")]
    [Authorize]
    public async Task<IActionResult> GetCurrentAsync()
    {
        var result = await _tenantService.GetCurrentAsync();

        return result.ToActionResult();
    }

    [HttpPost]
    public async Task<IActionResult> PostAsync([FromBody] UpsertTenantRequest request)
    {
        var result = await _tenantService.CreateAsync(request);

        return result.ToActionResult();
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> PutAsync(Guid id, [FromBody] UpsertTenantRequest request)
    {
        var result = await _tenantService.UpdateAsync(id, request);

        return result.ToActionResult();
    }
}
