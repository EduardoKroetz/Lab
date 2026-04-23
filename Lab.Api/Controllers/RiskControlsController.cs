using Lab.Api.Extensions;
using Lab.Application.DTOs.RiskControls;
using Lab.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Lab.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class RiskControlsController : ControllerBase
{
    private readonly RiskControlService _riskControlService;

    public RiskControlsController(RiskControlService riskControlService)
    {
        _riskControlService = riskControlService;
    }

    [HttpGet]
    public async Task<IActionResult> GetListAsync()
    {
        var result = await _riskControlService.GetListAsync();

        return result.ToActionResult();
    }

    [HttpGet("{id:guid}", Name = nameof(GetRiskControlByIdAsync))]
    public async Task<IActionResult> GetRiskControlByIdAsync(Guid id)
    {
        var result = await _riskControlService.GetByIdAsync(id);

        return result.ToActionResult();
    }

    [HttpPost]
    public async Task<IActionResult> PostAsync([FromBody] UpsertRiskControlRequest request)
    {
        var result = await _riskControlService.CreateAsync(request);
        if (!result.Succeeded)
            return result.ToActionResult();

        return CreatedAtRoute(nameof(GetRiskControlByIdAsync), new { id = result.Value!.Id }, result.Value);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> PutAsync(Guid id, [FromBody] UpsertRiskControlRequest request)
    {
        var result = await _riskControlService.UpdateAsync(id, request);

        return result.ToActionResult();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteAsync(Guid id)
    {
        var result = await _riskControlService.DeleteAsync(id);

        return result.ToActionResult();
    }
}
