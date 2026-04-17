using AutoMapper;
using Lab.Api.Application.DTOs.Appointments;
using Lab.Api.Domain.Entities;
using Lab.Api.Domain.Exceptions;
using Lab.Api.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Lab.Api.Application.Services;

public class AppointmentService
{
    private readonly LabDbContext _dbContext;
    private readonly IMapper _mapper;
    private readonly UserManager<ApplicationUser> _userManager;

    public AppointmentService(LabDbContext dbContext, IMapper mapper, UserManager<ApplicationUser> userManager)
    {
        _dbContext = dbContext;
        _mapper = mapper;
        _userManager = userManager;
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
        var customer = await _dbContext.Customers.FindAsync(dto.CustomerId);
        if (customer == null)
            throw new NotFoundException("Cliente não encontrado.");

        var offering = dto.OfferingId.HasValue ? await _dbContext.Offerings.FindAsync(dto.OfferingId.Value) : null;
        if (dto.OfferingId.HasValue && offering == null)
            throw new NotFoundException("Oferta não encontrada.");

        var user = await _userManager.FindByIdAsync(createdByUserId.ToString());
        if (user == null)
            throw new NotFoundException("Usuário criador não encontrado.");

        await ValidateAsync(dto);

        var appointment = new Appointment(dto.Name, dto.Description, dto.StartDate, dto.EndDate, customer, offering, user);

        await _dbContext.Appointments.AddAsync(appointment);
        await _dbContext.SaveChangesAsync();

        return await GetByIdAsync(appointment.Id);
    }

    public async Task<GetAppointmentDto> UpdateAsync(Guid id, UpsertAppointmentDto dto)
    {
        var customer = await _dbContext.Customers.FindAsync(dto.CustomerId);
        if (customer == null)
            throw new NotFoundException("Cliente não encontrado.");

        var offering = dto.OfferingId.HasValue ? await _dbContext.Offerings.FindAsync(dto.OfferingId.Value) : null;
        if (dto.OfferingId.HasValue && offering == null)
            throw new NotFoundException("Oferta não encontrada.");

        var appointment = await _dbContext.Appointments.FindAsync(id);
        if (appointment == null)
            throw new NotFoundException("Agendamento não encontrado.");

        await ValidateAsync(dto, id);

        appointment.Update(dto.Name, dto.Description, dto.StartDate, dto.EndDate, customer, offering);

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
