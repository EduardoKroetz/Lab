using Lab.Api.Extensions;
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
}
