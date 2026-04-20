using Lab.Api.Extensions;
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
    public async Task<IActionResult> GetCurrentAsync()
    {
        var tenantId = User.GetTenantId();

        var responseDto = await _tenantService.GetCurrentAsync(tenantId);

        return Ok(new ResponseDto(responseDto));
    }

    [HttpPost]
    public async Task<IActionResult> PostAsync([FromBody] UpsertTenantDto dto)
    {
        var responseDto = await _tenantService.CreateAsync(dto);

        return Ok(new ResponseDto(responseDto));
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> PutAsync(Guid id, [FromBody] UpsertTenantDto dto)
    {
        var responseDto = await _tenantService.UpdateAsync(id, dto);

        return Ok(new ResponseDto(responseDto));
    }
}
