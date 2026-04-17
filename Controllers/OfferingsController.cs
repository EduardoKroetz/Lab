using Lab.Api.Application.DTOs;
using Lab.Api.Application.DTOs.Offerings;
using Lab.Api.Application.Services;
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
        var dto = await _offeringService.GetListAsync();

        return Ok(new ResponseDto(dto));
    }

    [HttpGet("{id:guid}", Name = nameof(GetOfferingByIdAsync))]
    public async Task<IActionResult> GetOfferingByIdAsync(Guid id)
    {
        var dto = await _offeringService.GetByIdAsync(id);

        return Ok(new ResponseDto(dto));
    }

    [HttpPost]
    public async Task<IActionResult> PostAsync([FromBody] UpsertOfferingDto dto)
    {
        var responseDto = await _offeringService.CreateAsync(dto);

        return CreatedAtRoute(nameof(GetOfferingByIdAsync), new { id = responseDto.Id }, new ResponseDto(responseDto));
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> PutAsync(Guid id, [FromBody] UpsertOfferingDto dto)
    {
        var responseDto = await _offeringService.UpdateAsync(id, dto);

        return Ok(new ResponseDto(responseDto));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteAsync(Guid id)
    {
        await _offeringService.DeleteAsync(id);

        return NoContent();
    }
}
