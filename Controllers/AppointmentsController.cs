using Lab.Api.Application.DTOs;
using Lab.Api.Application.DTOs.Appointments;
using Lab.Api.Application.Services;
using Lab.Api.Common.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Lab.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AppointmentsController : ControllerBase
{
    private readonly AppointmentService _appointmentService;

    public AppointmentsController(AppointmentService appointmentService)
    {
        _appointmentService = appointmentService;
    }

    [HttpGet]
    public async Task<IActionResult> GetListAsync()
    {
        var dto = await _appointmentService.GetListAsync();

        return Ok(new ResponseDto(dto));
    }

    [HttpGet("{id:guid}", Name = nameof(GetAppointmentByIdAsync))]
    public async Task<IActionResult> GetAppointmentByIdAsync(Guid id)
    {
        var dto = await _appointmentService.GetByIdAsync(id);

        return Ok(new ResponseDto(dto));
    }

    [HttpPost]
    public async Task<IActionResult> PostAsync([FromBody] UpsertAppointmentDto dto)
    {
        var responseDto = await _appointmentService.CreateAsync(dto, User.GetUserId());

        return CreatedAtRoute(nameof(GetAppointmentByIdAsync), new { id = responseDto.Id }, new ResponseDto(responseDto));
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> PutAsync(Guid id, [FromBody] UpsertAppointmentDto dto)
    {
        var responseDto = await _appointmentService.UpdateAsync(id, dto);

        return Ok(new ResponseDto(responseDto));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteAsync(Guid id)
    {
        await _appointmentService.DeleteAsync(id);

        return NoContent();
    }
}
