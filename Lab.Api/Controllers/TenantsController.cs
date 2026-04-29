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

        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> PostAsync([FromBody] InsertTenantRequest request)
    {
        var result = await _tenantService.CreateAsync(request);

        return Ok(result);
    }

    [HttpPut("current")]
    public async Task<IActionResult> PutAsync([FromBody] UpdateCurrentTenantRequest request)
    {
        var result = await _tenantService.UpdateCurrentAsync(request);

        return Ok(result);
    }
}
