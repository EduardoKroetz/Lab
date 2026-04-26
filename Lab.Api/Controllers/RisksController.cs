using Lab.Api.Extensions;
using Lab.Application.DTOs.RiskControls;
using Lab.Application.DTOs.Risks;
using Lab.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Lab.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class RisksController : ControllerBase
{
    private readonly RiskService _riskService;

    public RisksController(RiskService riskService)
    {
        _riskService = riskService;
    }

    [HttpGet]
    public async Task<IActionResult> GetListAsync()
    {
        var result = await _riskService.GetListAsync();

        return result.ToActionResult();
    }

    [HttpGet("{id:guid}", Name = nameof(GetRiskByIdAsync))]
    public async Task<IActionResult> GetRiskByIdAsync(Guid id)
    {
        var result = await _riskService.GetByIdAsync(id);

        return result.ToActionResult();
    }

    [HttpPost]
    public async Task<IActionResult> PostAsync([FromBody] InsertRiskRequest request)
    {
        var result = await _riskService.CreateAsync(request);
        if (!result.Succeeded)
            return result.ToActionResult();

        return CreatedAtRoute(nameof(GetRiskByIdAsync), new { id = result.Value!.Id }, result.Value);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> PutAsync(Guid id, [FromBody] UpdateRiskRequest request)
    {
        var result = await _riskService.UpdateAsync(id, request);

        return result.ToActionResult();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteAsync(Guid id)
    {
        var result = await _riskService.DeleteAsync(id);

        return result.ToActionResult();
    }

    [HttpPost("{riskId:guid}/controls")]
    public async Task<IActionResult> AddControlAsync([FromRoute] Guid riskId, [FromBody] InsertRiskControlRequest request)
    {
        var result = await _riskService.AddControlAsync(riskId, request);

        return result.ToActionResult();
    }

    [HttpDelete("{riskId:guid}/controls/{controlId:guid}")]
    public async Task<IActionResult> RemoveControlAsync([FromRoute] Guid riskId, [FromRoute] Guid controlId)
    {
        var result = await _riskService.RemoveControlAsync(riskId, controlId);

        return result.ToActionResult();
    }

    [HttpPatch("{riskId:guid}/controls/effectiveness")]
    public async Task<IActionResult> ChangeControlEffectivenessAsync([FromRoute] Guid riskId, [FromBody] UpdateRiskControlEffectivenessRequest request)
    {
        var result = await _riskService.ChangeControlEffectivenessAsync(riskId, request);

        return result.ToActionResult();
    }

    [HttpGet("{riskId:guid}/controls")]
    public async Task<IActionResult> GetListControlsAsync([FromRoute] Guid riskId)
    {
        var result = await _riskService.GetListControlsAsync(riskId);

        return result.ToActionResult();
    }
}
