using AutoMapper;
using Lab.Application.Common.Interfaces;
using Lab.Application.DTOs.RiskControls;
using Lab.Domain.Common.Models;
using Lab.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Lab.Application.Services;

public class RiskControlService
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IMapper _mapper;

    public RiskControlService(IApplicationDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<Result<List<GetRiskControlResponse>>> GetListAsync()
    {
        var riskControls = await _dbContext.RiskControls
            .AsNoTracking()
            .Include(riskControl => riskControl.Risk)
            .Include(riskControl => riskControl.Control)
            .ToListAsync();

        var responses = riskControls.Select(riskControl => _mapper.Map<GetRiskControlResponse>(riskControl)).ToList();

        return Result<List<GetRiskControlResponse>>.Success(responses);
    }

    public async Task<Result<GetRiskControlResponse>> GetByIdAsync(Guid id)
    {
        var riskControl = await _dbContext.RiskControls
            .AsNoTracking()
            .Include(riskControl => riskControl.Risk)
            .Include(riskControl => riskControl.Control)
            .FirstOrDefaultAsync(riskControl => riskControl.Id == id);

        if (riskControl == null)
            return Result<GetRiskControlResponse>.Failure("Vínculo de risco e controle não encontrado.");

        return Result<GetRiskControlResponse>.Success(_mapper.Map<GetRiskControlResponse>(riskControl));
    }

    public async Task<Result<GetRiskControlResponse>> CreateAsync(UpsertRiskControlRequest request)
    {
        var risk = await _dbContext.Risks.FindAsync(request.RiskId);
        if (risk == null)
            return Result<GetRiskControlResponse>.Failure("Risco não encontrado.");

        var control = await _dbContext.Controls.FindAsync(request.ControlId);
        if (control == null)
            return Result<GetRiskControlResponse>.Failure("Controle não encontrado.");

        var riskControl = new RiskControl(risk, control, request.Effectiveness);

        await _dbContext.RiskControls.AddAsync(riskControl);
        await _dbContext.SaveChangesAsync();

        return await GetByIdAsync(riskControl.Id);
    }

    public async Task<Result<GetRiskControlResponse>> UpdateAsync(Guid id, UpsertRiskControlRequest request)
    {
        var risk = await _dbContext.Risks.FindAsync(request.RiskId);
        if (risk == null)
            return Result<GetRiskControlResponse>.Failure("Risco não encontrado.");

        var control = await _dbContext.Controls.FindAsync(request.ControlId);
        if (control == null)
            return Result<GetRiskControlResponse>.Failure("Controle não encontrado.");

        var riskControl = await _dbContext.RiskControls.FindAsync(id);
        if (riskControl == null)
            return Result<GetRiskControlResponse>.Failure("Vínculo de risco e controle não encontrado.");

        riskControl.Update(risk, control, request.Effectiveness);

        await _dbContext.SaveChangesAsync();

        return await GetByIdAsync(id);
    }

    public async Task<Result> DeleteAsync(Guid id)
    {
        var riskControl = await _dbContext.RiskControls.FindAsync(id);
        if (riskControl == null)
            return Result.Failure("Vínculo de risco e controle não encontrado.");

        _dbContext.RiskControls.Remove(riskControl);
        await _dbContext.SaveChangesAsync();

        return Result.Success();
    }
}
