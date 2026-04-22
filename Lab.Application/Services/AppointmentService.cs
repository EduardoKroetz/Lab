using AutoMapper;
using Lab.Application.Common.Interfaces;
using Lab.Application.DTOs.Appointments;
using Lab.Domain.Common.Models;
using Lab.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Lab.Application.Services;

public class AppointmentService
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IMapper _mapper;
    private readonly IUserProvider _userProvider;

    public AppointmentService(IApplicationDbContext dbContext, IMapper mapper, IUserProvider userProvider)
    {
        _dbContext = dbContext;
        _mapper = mapper;
        _userProvider = userProvider;
    }

    public async Task<Result<List<GetAppointmentResponse>>> GetListAsync()
    {
        var appointments = await _dbContext.Appointments
            .AsNoTracking()
            .Include(a => a.Customer)
            .Include(a => a.Offering)
            .ToListAsync();

        var appointmentDtos = appointments.Select(a => _mapper.Map<GetAppointmentResponse>(a)).ToList();

        return Result<List<GetAppointmentResponse>>.Success(appointmentDtos);
    }

    public async Task<Result<GetAppointmentResponse>> GetByIdAsync(Guid id)
    {
        var appointment = await _dbContext.Appointments
            .AsNoTracking()
            .Include(a => a.Customer)
            .Include(a => a.Offering)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (appointment == null)
            return Result<GetAppointmentResponse>.Failure("Agendamento não encontrado.");

        return Result<GetAppointmentResponse>.Success(_mapper.Map<GetAppointmentResponse>(appointment));
    }

    public async Task<Result<GetAppointmentResponse>> CreateAsync(UpsertAppointmentRequest request)
    {
        var customer = await _dbContext.Customers.FindAsync(request.CustomerId);
        if (customer == null)
            return Result<GetAppointmentResponse>.Failure("Cliente não encontrado.");

        var offering = request.OfferingId.HasValue
            ? await _dbContext.Offerings.FindAsync(request.OfferingId.Value)
            : null;

        if (request.OfferingId.HasValue && offering == null)
            return Result<GetAppointmentResponse>.Failure("Oferta não encontrada.");

        var validationResult = await ValidateAsync(request);
        if (!validationResult.Succeeded)
            return Result<GetAppointmentResponse>.Failure(validationResult.Errors);

        var appointment = new Appointment(
            request.Name,
            request.Description,
            request.StartDate,
            request.EndDate,
            customer,
            offering,
            createdBy: _userProvider.UserId);

        await _dbContext.Appointments.AddAsync(appointment);
        await _dbContext.SaveChangesAsync();

        return await GetByIdAsync(appointment.Id);
    }

    public async Task<Result<GetAppointmentResponse>> UpdateAsync(Guid id, UpsertAppointmentRequest request)
    {
        var customer = await _dbContext.Customers.FindAsync(request.CustomerId);
        if (customer == null)
            return Result<GetAppointmentResponse>.Failure("Cliente não encontrado.");

        var offering = request.OfferingId.HasValue
            ? await _dbContext.Offerings.FindAsync(request.OfferingId.Value)
            : null;

        if (request.OfferingId.HasValue && offering == null)
            return Result<GetAppointmentResponse>.Failure("Oferta não encontrada.");

        var appointment = await _dbContext.Appointments.FindAsync(id);
        if (appointment == null)
            return Result<GetAppointmentResponse>.Failure("Agendamento não encontrado.");

        var validationResult = await ValidateAsync(request, id);
        if (!validationResult.Succeeded)
            return Result<GetAppointmentResponse>.Failure(validationResult.Errors);

        appointment.Update(request.Name, request.Description, request.StartDate, request.EndDate, customer, offering);

        await _dbContext.SaveChangesAsync();

        return await GetByIdAsync(id);
    }

    public async Task<Result> DeleteAsync(Guid id)
    {
        var appointment = await _dbContext.Appointments.FindAsync(id);
        if (appointment == null)
            return Result.Failure("Agendamento não encontrado.");

        _dbContext.Appointments.Remove(appointment);
        await _dbContext.SaveChangesAsync();

        return Result.Success();
    }

    private async Task<Result> ValidateAsync(UpsertAppointmentRequest request, Guid? appointmentId = null)
    {
        var scheduledAppointment = await _dbContext.Appointments.FirstOrDefaultAsync(a =>
            a.Id != appointmentId &&
            request.StartDate < a.EndDate &&
            request.EndDate > a.StartDate
        );

        if (scheduledAppointment != null)
        {
            var message = appointmentId.HasValue
                ? $"Já existe um agendamento para o período informado. Agendamento conflitante: {scheduledAppointment.StartDate:HH:mm} - {scheduledAppointment.EndDate:HH:mm}"
                : $"Já existe um agendamento para o período informado. Período conflitante: {scheduledAppointment.StartDate:HH:mm} - {scheduledAppointment.EndDate:HH:mm}";

            return Result.Failure(message);
        }

        return Result.Success();
    }
}
