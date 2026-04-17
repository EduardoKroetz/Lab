using Lab.Api.Application.Services;
using Lab.Api.Domain.Entities;
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

    [HttpPost]
    public async Task<IActionResult> PostAsync(Tenant tenant)
    {
        await _tenantService.CreateAsync(tenant);

        return Ok();
    }
}
