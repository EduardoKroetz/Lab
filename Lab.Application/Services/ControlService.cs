using AutoMapper;
using Lab.Application.Common.Interfaces;
using Lab.Application.Common.Models;
using Lab.Application.DTOs.Controls;
using Lab.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Lab.Application.Services;

public class ControlService
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IMapper _mapper;

    public ControlService(IApplicationDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<Result<List<GetControlResponse>>> GetListAsync()
    {
        var controls = await _dbContext.Controls
            .AsNoTracking()
            .ToListAsync();

        var responses = controls.Select(control => _mapper.Map<GetControlResponse>(control)).ToList();

        return Result<List<GetControlResponse>>.Success(responses);
    }

    public async Task<Result<GetControlResponse>> GetByIdAsync(Guid id)
    {
        var control = await _dbContext.Controls
            .AsNoTracking()
            .FirstOrDefaultAsync(control => control.Id == id);

        if (control == null)
            return Result<GetControlResponse>.Failure("Controle não encontrado.");

        return Result<GetControlResponse>.Success(_mapper.Map<GetControlResponse>(control));
    }

    public async Task<Result<GetControlResponse>> CreateAsync(UpsertControlRequest request)
    {
        var control = new Control(request.Name, request.Description, request.Type, request.Category);

        await _dbContext.Controls.AddAsync(control);
        await _dbContext.SaveChangesAsync();

        return await GetByIdAsync(control.Id);
    }

    public async Task<Result<GetControlResponse>> UpdateAsync(Guid id, UpsertControlRequest request)
    {
        var control = await _dbContext.Controls.FindAsync(id);
        if (control == null)
            return Result<GetControlResponse>.Failure("Controle não encontrado.");

        control.Update(request.Name, request.Description, request.Type, request.Category);

        await _dbContext.SaveChangesAsync();

        return await GetByIdAsync(id);
    }

    public async Task<Result> DeleteAsync(Guid id)
    {
        var control = await _dbContext.Controls.FindAsync(id);
        if (control == null)
            return Result.Failure("Controle não encontrado.");

        _dbContext.Controls.Remove(control);
        await _dbContext.SaveChangesAsync();

        return Result.Success();
    }
}
