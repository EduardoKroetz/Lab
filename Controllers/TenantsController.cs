using Lab.Api.Application.DTOs;
using Lab.Api.Application.DTOs.Tenants;
using Lab.Api.Application.Services;
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
    public async Task<IActionResult> PostAsync([FromBody] UpsertTenantDto dto)
    {
        var responseDto = await _tenantService.CreateAsync(dto);

        return Ok(new ResponseDto(responseDto));
    }
}
