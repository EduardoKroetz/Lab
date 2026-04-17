using AutoMapper;
using Lab.Api.Application.DTOs.Appointments;
using Lab.Api.Domain.Entities;
using Lab.Api.Domain.Exceptions;
using Lab.Api.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Lab.Api.Application.Services;

public class AppointmentService
{
    private readonly LabDbContext _dbContext;
    private readonly IMapper _mapper;

    public AppointmentService(LabDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<List<GetAppointmentDto>> GetListAsync()
    {
        var appointments = await _dbContext.Appointments
            .AsNoTracking()
            .Include(a => a.Customer)
            .Include(a => a.Offering)
            .Include(a => a.CreatedByUser)
            .ToListAsync();

        return appointments.Select(a => _mapper.Map<GetAppointmentDto>(a)).ToList();
    }

    public async Task<GetAppointmentDto> GetByIdAsync(Guid id)
    {
        var appointment = await _dbContext.Appointments
            .AsNoTracking()
            .Include(a => a.Customer)
            .Include(a => a.Offering)
            .Include(a => a.CreatedByUser)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (appointment == null)
            throw new NotFoundException("Agendamento não encontrado.");

        return _mapper.Map<GetAppointmentDto>(appointment);
    }

    public async Task<GetAppointmentDto> CreateAsync(UpsertAppointmentDto dto, Guid createdByUserId)
    {
        await ValidateAsync(dto);

        var appointment = new Appointment
        {
            Name = dto.Name,
            Description = dto.Description,
            CustomerId = dto.CustomerId,
            OfferingId = dto.OfferingId,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            CreatedBy = createdByUserId,
        };

        await _dbContext.Appointments.AddAsync(appointment);
        await _dbContext.SaveChangesAsync();

        return await GetByIdAsync(appointment.Id);
    }

    public async Task<GetAppointmentDto> UpdateAsync(Guid id, UpsertAppointmentDto dto)
    {
        var appointment = await _dbContext.Appointments.FindAsync(id);
        if (appointment == null)
            throw new NotFoundException("Agendamento não encontrado.");

        await ValidateAsync(dto, id);

        appointment.Name = dto.Name;
        appointment.Description = dto.Description;
        appointment.CustomerId = dto.CustomerId;
        appointment.OfferingId = dto.OfferingId;
        appointment.StartDate = dto.StartDate;
        appointment.EndDate = dto.EndDate;

        await _dbContext.SaveChangesAsync();

        return await GetByIdAsync(id);
    }

    public async Task DeleteAsync(Guid id)
    {
        var appointment = await _dbContext.Appointments.FindAsync(id);
        if (appointment == null)
            throw new NotFoundException("Agendamento não encontrado.");

        _dbContext.Appointments.Remove(appointment);
        await _dbContext.SaveChangesAsync();
    }

    private async Task ValidateAsync(UpsertAppointmentDto dto, Guid? appointmentId = null)
    {
        if (!await _dbContext.Customers.AnyAsync(c => c.Id == dto.CustomerId))
            throw new BadRequestException("Cliente não encontrado.");

        if (dto.OfferingId.HasValue && !await _dbContext.Offerings.AnyAsync(o => o.Id == dto.OfferingId.Value))
            throw new BadRequestException("Oferta não encontrada.");

        if (dto.StartDate >= dto.EndDate)
            throw new BadRequestException("A data de início deve ser anterior à data de término.");

        var scheduledAppointment = await _dbContext.Appointments.FirstOrDefaultAsync(a =>
            a.Id != appointmentId &&
            dto.StartDate < a.EndDate &&
            dto.EndDate > a.StartDate
        );

        if (scheduledAppointment != null)
        {
            var message = appointmentId.HasValue
                ? $"Já existe um agendamento para o período informado. Agendamento conflitante: {scheduledAppointment.StartDate:HH:mm} - {scheduledAppointment.EndDate:HH:mm}"
                : $"Já existe um agendamento para o período informado. Período conflitante: {scheduledAppointment.StartDate:HH:mm} - {scheduledAppointment.EndDate:HH:mm}";

            throw new BadRequestException(message);
        }
    }
}
