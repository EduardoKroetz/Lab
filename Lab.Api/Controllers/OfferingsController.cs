using Lab.Api.Extensions;
using Lab.Application.DTOs.Offerings;
using Lab.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Lab.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OfferingsController : ControllerBase
{
    private readonly OfferingService _offeringService;

    public OfferingsController(OfferingService offeringService)
    {
        _offeringService = offeringService;
    }

    [HttpGet]
    public async Task<IActionResult> GetListAsync()
    {
        var result = await _offeringService.GetListAsync();

        return result.ToActionResult();
    }

    [HttpGet("{id:guid}", Name = nameof(GetOfferingByIdAsync))]
    public async Task<IActionResult> GetOfferingByIdAsync(Guid id)
    {
        var result = await _offeringService.GetByIdAsync(id);

        return result.ToActionResult();
    }

    [HttpPost]
    public async Task<IActionResult> PostAsync([FromBody] UpsertOfferingRequest request)
    {
        var result = await _offeringService.CreateAsync(request);
        if (!result.Succeeded)
            return result.ToActionResult();

        return CreatedAtRoute(nameof(GetOfferingByIdAsync), new { id = result.Value!.Id }, result.Value);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> PutAsync(Guid id, [FromBody] UpsertOfferingRequest request)
    {
        var result = await _offeringService.UpdateAsync(id, request);

        return result.ToActionResult();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteAsync(Guid id)
    {
        var result = await _offeringService.DeleteAsync(id);

        return result.ToActionResult();
    }
}
