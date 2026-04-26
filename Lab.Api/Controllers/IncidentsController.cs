using Lab.Api.Extensions;
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

        return result.ToActionResult();
    }

    [HttpGet("{id:guid}", Name = nameof(GetIncidentByIdAsync))]
    public async Task<IActionResult> GetIncidentByIdAsync(Guid id)
    {
        var result = await _incidentService.GetByIdAsync(id);

        return result.ToActionResult();
    }

    [HttpPost]
    public async Task<IActionResult> PostAsync([FromBody] UpsertIncidentRequest request)
    {
        var result = await _incidentService.CreateAsync(request);
        if (!result.Succeeded)
            return result.ToActionResult();

        return CreatedAtRoute(nameof(GetIncidentByIdAsync), new { id = result.Value!.Id }, result.Value);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> PutAsync(Guid id, [FromBody] UpsertIncidentRequest request)
    {
        var result = await _incidentService.UpdateAsync(id, request);

        return result.ToActionResult();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteAsync(Guid id)
    {
        var result = await _incidentService.DeleteAsync(id);

        return result.ToActionResult();
    }

    [HttpPost("{incidentId:guid}/impacts")]
    public async Task<IActionResult> AddImpactAsync([FromRoute] Guid incidentId, [FromBody] UpsertIncidentImpactRequest request)
    {
        var result = await _incidentService.AddImpactAsync(incidentId, request);

        return result.ToActionResult();
    }

    [HttpDelete("{incidentId:guid}/impacts/{impactId:guid}")]
    public async Task<IActionResult> RemoveImpactAsync([FromRoute] Guid incidentId, [FromRoute] Guid impactId)
    {
        var result = await _incidentService.RemoveImpactAsync(incidentId, impactId);

        return result.ToActionResult();
    }

    [HttpGet("{incidentId:guid}/impacts")]
    public async Task<IActionResult> GetListImpactsAsync([FromRoute] Guid incidentId)
    {
        var result = await _incidentService.GetListImpactsAsync(incidentId);

        return result.ToActionResult();
    }
}
