using AutoMapper;
using Lab.Application.Common.Interfaces;
using Lab.Application.DTOs.Controls;
using Lab.Domain.Entities;
using Lab.Domain.Exceptions;
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

    public async Task<List<GetControlResponse>> GetListAsync()
    {
        var controls = await _dbContext.Controls
            .AsNoTracking()
            .ToListAsync();

        return controls.Select(control => _mapper.Map<GetControlResponse>(control)).ToList();
    }

    public async Task<GetControlResponse> GetByIdAsync(Guid id)
    {
        var control = await _dbContext.Controls
            .AsNoTracking()
            .FirstOrDefaultAsync(control => control.Id == id);

        if (control == null)
            throw new NotFoundException("Controle não encontrado.");

        return _mapper.Map<GetControlResponse>(control);
    }

    public async Task<GetControlResponse> CreateAsync(UpsertControlRequest request)
    {
        var control = new Control(request.Name, request.Description, request.Type, request.Category);

        await _dbContext.Controls.AddAsync(control);
        await _dbContext.SaveChangesAsync();

        return await GetByIdAsync(control.Id);
    }

    public async Task<GetControlResponse> UpdateAsync(Guid id, UpsertControlRequest request)
    {
        var control = await _dbContext.Controls.FindAsync(id);
        if (control == null)
            throw new NotFoundException("Controle não encontrado.");

        control.Update(request.Name, request.Description, request.Type, request.Category);

        await _dbContext.SaveChangesAsync();

        return await GetByIdAsync(id);
    }

    public async Task DeleteAsync(Guid id)
    {
        var control = await _dbContext.Controls.FindAsync(id);
        if (control == null)
            throw new NotFoundException("Controle não encontrado.");

        _dbContext.Controls.Remove(control);
        await _dbContext.SaveChangesAsync();
    }
}
