using Lab.Api.Extensions;
using Lab.Application.DTOs.IncidentImpacts;
using Lab.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Lab.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class IncidentImpactsController : ControllerBase
{
    private readonly IncidentImpactService _incidentImpactService;

    public IncidentImpactsController(IncidentImpactService incidentImpactService)
    {
        _incidentImpactService = incidentImpactService;
    }

    [HttpGet]
    public async Task<IActionResult> GetListAsync()
    {
        var result = await _incidentImpactService.GetListAsync();

        return result.ToActionResult();
    }

    [HttpGet("{id:guid}", Name = nameof(GetIncidentImpactByIdAsync))]
    public async Task<IActionResult> GetIncidentImpactByIdAsync(Guid id)
    {
        var result = await _incidentImpactService.GetByIdAsync(id);

        return result.ToActionResult();
    }

    [HttpPost]
    public async Task<IActionResult> PostAsync([FromBody] UpsertIncidentImpactRequest request)
    {
        var result = await _incidentImpactService.CreateAsync(request);
        if (!result.Succeeded)
            return result.ToActionResult();

        return CreatedAtRoute(nameof(GetIncidentImpactByIdAsync), new { id = result.Value!.Id }, result.Value);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> PutAsync(Guid id, [FromBody] UpsertIncidentImpactRequest request)
    {
        var result = await _incidentImpactService.UpdateAsync(id, request);

        return result.ToActionResult();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteAsync(Guid id)
    {
        var result = await _incidentImpactService.DeleteAsync(id);

        return result.ToActionResult();
    }
}
