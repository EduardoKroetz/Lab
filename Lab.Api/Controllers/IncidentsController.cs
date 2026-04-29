using Lab.Application.DTOs.IncidentImpacts;
using Lab.Application.DTOs.Incidents;
using Lab.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Lab.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class IncidentsController : ControllerBase
{
    private readonly IncidentService _incidentService;

    public IncidentsController(IncidentService incidentService)
    {
        _incidentService = incidentService;
    }

    [HttpGet]
    public async Task<IActionResult> GetListAsync()
    {
        var result = await _incidentService.GetListAsync();

        return Ok(result);
    }

    [HttpGet("{id:guid}", Name = nameof(GetIncidentByIdAsync))]
    public async Task<IActionResult> GetIncidentByIdAsync(Guid id)
    {
        var result = await _incidentService.GetByIdAsync(id);

        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> PostAsync([FromBody] UpsertIncidentRequest request)
    {
        var result = await _incidentService.CreateAsync(request);

        return CreatedAtRoute(nameof(GetIncidentByIdAsync), new { id = result.Id }, result);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> PutAsync(Guid id, [FromBody] UpsertIncidentRequest request)
    {
        var result = await _incidentService.UpdateAsync(id, request);

        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteAsync(Guid id)
    {
        await _incidentService.DeleteAsync(id);

        return NoContent();
    }

    [HttpPost("{incidentId:guid}/impacts")]
    public async Task<IActionResult> AddImpactAsync([FromRoute] Guid incidentId, [FromBody] UpsertIncidentImpactRequest request)
    {
        await _incidentService.AddImpactAsync(incidentId, request);

        return NoContent();
    }

    [HttpDelete("{incidentId:guid}/impacts/{impactId:guid}")]
    public async Task<IActionResult> RemoveImpactAsync([FromRoute] Guid incidentId, [FromRoute] Guid impactId)
    {
        await _incidentService.RemoveImpactAsync(incidentId, impactId);

        return NoContent();
    }

    [HttpGet("{incidentId:guid}/impacts")]
    public async Task<IActionResult> GetListImpactsAsync([FromRoute] Guid incidentId)
    {
        var result = await _incidentService.GetListImpactsAsync(incidentId);

        return Ok(result);
    }
}
