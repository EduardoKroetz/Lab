using Lab.Api.Data;
using Lab.Api.DTOs;
using Lab.Api.DTOs.Appointments;
using Lab.Api.Entities;
using Lab.Api.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Lab.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AppointmentsController : ControllerBase
{
    private readonly LabDbContext _dbContext;

    public AppointmentsController(LabDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet]
    public async Task<IActionResult> GetListAsync()
    {
        var appointments = await _dbContext.Appointments
            .AsNoTracking()
            .Include(a => a.Customer)
            .Include(a => a.Service)
            .Include(a => a.CreatedByUser)
            .ToListAsync();

        var dto = appointments.Select(a => new GetAppointmentDto(
            a.Id, 
            a.Name, 
            a.Description, 
            a.CustomerId, 
            a.Customer.Name, 
            a.ServiceId, 
            a.Service?.Name, 
            a.StartDate, 
            a.EndDate, 
            a.CreatedBy, 
            a.CreatedByUser?.UserName))
        .ToList();

        return Ok(new ResponseDto(dto));
    }

    [HttpGet("{id:guid}", Name = nameof(GetAppointmentByIdAsync))]
    public async Task<IActionResult> GetAppointmentByIdAsync(Guid id)
    {
        var appointment = await _dbContext.Appointments
            .AsNoTracking()
            .Include(a => a.Customer)
            .Include(a => a.Service)
            .Include(a => a.CreatedByUser)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (appointment == null)
            return NotFound(new ResponseDto("Agendamento não encontrado."));

        var dto = new GetAppointmentDto(
            appointment.Id, 
            appointment.Name, 
            appointment.Description, 
            appointment.CustomerId, 
            appointment.Customer.Name, 
            appointment.ServiceId, 
            appointment.Service?.Name, 
            appointment.StartDate, 
            appointment.EndDate, 
            appointment.CreatedBy, 
            appointment.CreatedByUser?.UserName
        );

        return Ok(new ResponseDto(dto));
    }

    [HttpPost]
    public async Task<IActionResult> PostAsync([FromBody] UpsertAppointmentDto dto)
    {
        if (!await _dbContext.Customers.AnyAsync(c => c.Id == dto.CustomerId))
            return BadRequest(new ResponseDto("Cliente não encontrado."));

        if (dto.ServiceId.HasValue && !await _dbContext.Services.AnyAsync(s => s.Id == dto.ServiceId.Value))
            return BadRequest(new ResponseDto("Serviço não encontrado."));

        if (dto.StartDate >= dto.EndDate || dto.StartDate == dto.EndDate)
            return BadRequest(new ResponseDto("A data de início deve ser anterior à data de término."));

        var scheduledAppointment = await _dbContext.Appointments.FirstOrDefaultAsync(a =>
            (dto.StartDate == a.StartDate) ||
            (dto.EndDate == a.EndDate) ||
            (dto.StartDate > a.StartDate && dto.StartDate < a.EndDate) || // Data inicial está dentro de um período
            (dto.EndDate > a.StartDate && dto.EndDate < a.EndDate) || // Data final está dentro de um período
            (dto.StartDate < a.StartDate && dto.EndDate > a.EndDate) // Período está dentro de outro período
        );
            
        if (scheduledAppointment != null)
            return BadRequest(new ResponseDto($"Já existe um agendamento para o período informado. Período conflitante: {scheduledAppointment.StartDate:HH:mm} - {scheduledAppointment.EndDate:HH:mm}"));

        var appointment = new Appointment
        {  
            Name = dto.Name,
            Description = dto.Description,
            CustomerId = dto.CustomerId,
            ServiceId = dto.ServiceId,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            CreatedBy = User.GetUserId(),
        };

        await _dbContext.Appointments.AddAsync(appointment);
        await _dbContext.SaveChangesAsync();

        return CreatedAtRoute(nameof(GetAppointmentByIdAsync), new { id = appointment.Id }, new ResponseDto(appointment));
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> PutAsync(Guid id, [FromBody] UpsertAppointmentDto dto)
    {
        if (!await _dbContext.Customers.AnyAsync(c => c.Id == dto.CustomerId))
            return BadRequest(new ResponseDto("Cliente não encontrado."));

        if (dto.ServiceId.HasValue && !await _dbContext.Services.AnyAsync(s => s.Id == dto.ServiceId.Value))
            return BadRequest(new ResponseDto("Serviço não encontrado."));

        var appointment = await _dbContext.Appointments.FindAsync(id);
        if (appointment == null)
            return NotFound(new ResponseDto("Agendamento não encontrado."));

        if (dto.StartDate >= dto.EndDate || dto.StartDate == dto.EndDate)
            return BadRequest(new ResponseDto("A data de início deve ser anterior à data de término."));

        var scheduledAppointment = await _dbContext.Appointments.FirstOrDefaultAsync(a =>
            a.Id != id &&
            ((dto.StartDate == a.StartDate) ||
            (dto.EndDate == a.EndDate) ||
            (dto.StartDate > a.StartDate && dto.StartDate < a.EndDate) || // Data inicial está dentro de um período
            (dto.EndDate > a.StartDate && dto.EndDate < a.EndDate) || // Data final está dentro de um período
            (dto.StartDate < a.StartDate && dto.EndDate > a.EndDate)) // Período está dentro de outro período
        );

        if (scheduledAppointment != null)
            return BadRequest(new ResponseDto($"Já existe um agendamento para o período informado. Agendamento conflitante: {scheduledAppointment.StartDate:HH:mm} - {scheduledAppointment.EndDate:HH:mm}"));

        appointment.Name = dto.Name;
        appointment.Description = dto.Description;
        appointment.CustomerId = dto.CustomerId;
        appointment.ServiceId = dto.ServiceId;
        appointment.StartDate = dto.StartDate;
        appointment.EndDate = dto.EndDate;

        await _dbContext.SaveChangesAsync();

        return Ok(new ResponseDto(appointment));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteAsync(Guid id)
    {
        var service = await _dbContext.Appointments.FindAsync(id);
        if (service == null)
            return NotFound(new ResponseDto("Serviço não encontrado."));

        _dbContext.Appointments.Remove(service);

        await _dbContext.SaveChangesAsync();

        return NoContent();
    }
}
