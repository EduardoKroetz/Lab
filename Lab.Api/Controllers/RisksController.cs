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

        return Ok(result);
    }

    [HttpGet("{id:guid}", Name = nameof(GetRiskByIdAsync))]
    public async Task<IActionResult> GetRiskByIdAsync(Guid id)
    {
        var result = await _riskService.GetByIdAsync(id);

        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> PostAsync([FromBody] InsertRiskRequest request)
    {
        var result = await _riskService.CreateAsync(request);

        return CreatedAtRoute(nameof(GetRiskByIdAsync), new { id = result.Id }, result);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> PutAsync(Guid id, [FromBody] UpdateRiskRequest request)
    {
        var result = await _riskService.UpdateAsync(id, request);

        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteAsync(Guid id)
    {
        await _riskService.DeleteAsync(id);

        return NoContent();
    }

    [HttpPost("{riskId:guid}/controls")]
    public async Task<IActionResult> AddControlAsync([FromRoute] Guid riskId, [FromBody] InsertRiskControlRequest request)
    {
        await _riskService.AddControlAsync(riskId, request);

        return NoContent();
    }

    [HttpDelete("{riskId:guid}/controls/{controlId:guid}")]
    public async Task<IActionResult> RemoveControlAsync([FromRoute] Guid riskId, [FromRoute] Guid controlId)
    {
        await _riskService.RemoveControlAsync(riskId, controlId);

        return NoContent();
    }

    [HttpPatch("{riskId:guid}/controls/effectiveness")]
    public async Task<IActionResult> ChangeControlEffectivenessAsync([FromRoute] Guid riskId, [FromBody] UpdateRiskControlEffectivenessRequest request)
    {
        await _riskService.ChangeControlEffectivenessAsync(riskId, request);

        return NoContent();
    }

    [HttpGet("{riskId:guid}/controls")]
    public async Task<IActionResult> GetListControlsAsync([FromRoute] Guid riskId)
    {
        var result = await _riskService.GetListControlsAsync(riskId);

        return Ok(result);
    }
}
