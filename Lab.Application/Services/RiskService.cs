using AutoMapper;
using AutoMapper.QueryableExtensions;
using Lab.Application.Common.Interfaces;
using Lab.Application.Common.Models;
using Lab.Application.DTOs.RiskControls;
using Lab.Application.DTOs.Risks;
using Lab.Domain.Entities;
using Lab.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Lab.Application.Services;

public class RiskService
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IMapper _mapper;

    public RiskService(IApplicationDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    private async Task<bool> RiskCombinationExistsAsync(Guid assetId, Guid threatId, Guid vulnerabilityId, Guid? riskId = null)
    {
        var alreadyExists = await _dbContext.Risks.AnyAsync(r =>
            (r.Id != riskId) &&
            (r.AssetId == assetId &&
            r.ThreatId == threatId &&
            r.VulnerabilityId == vulnerabilityId)
        );

        return alreadyExists;
    }

    public async Task<Result> ChangeTreatmentAsync(Guid riskId, ERiskTreatment? treatment, string? description)
    {
        var risk = await _dbContext.Risks.Include(r => r.RiskControls).FirstOrDefaultAsync(r => r.Id == riskId);
        if (risk == null)
            return Result.Failure("Risco não encontrado");

        if (treatment == ERiskTreatment.Eliminate)
        {
            var asset = await _dbContext.Assets.FirstOrDefaultAsync(a => a.Id == risk.AssetId);
            if (asset == null)
                return Result.Failure("Ativo não encontrado");

            if (asset.Enabled)
                return Result.Failure("Para Eliminar, o ativo vinculado deve estar desabilitado.");
        }

        risk.ChangeTreatment(treatment, description);

        await _dbContext.SaveChangesAsync();

        return Result.Success();
    }

    public async Task<Result<List<GetRiskListResponse>>> GetListAsync()
    {
        var risks = await _dbContext.Risks
            .AsNoTracking()
            .Include(x => x.RiskControls)
            .ProjectTo<GetRiskListResponse>(_mapper.ConfigurationProvider)
            .ToListAsync();

        return Result<List<GetRiskListResponse>>.Success(risks);
    }

    public async Task<Result<GetRiskDetailResponse>> GetByIdAsync(Guid id)
    {
        var risk = await _dbContext.Risks
            .AsNoTracking()
            .Include(x => x.RiskControls)
            .ProjectTo<GetRiskDetailResponse>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(risk => risk.Id == id);

        if (risk == null)
            return Result<GetRiskDetailResponse>.Failure("Risco não encontrado.");

        return Result<GetRiskDetailResponse>.Success(risk);
    }

    public async Task<Result<GetRiskDetailResponse>> CreateAsync(InsertRiskRequest request)
    {
        var asset = await _dbContext.Assets.FindAsync(request.AssetId);
        if (asset == null)
            return Result<GetRiskDetailResponse>.Failure("Ativo não encontrado.");

        var threat = await _dbContext.Threats.FindAsync(request.ThreatId);
        if (threat == null)
            return Result<GetRiskDetailResponse>.Failure("Ameaça não encontrada.");

        var vulnerability = await _dbContext.Vulnerabilities.FindAsync(request.VulnerabilityId);
        if (vulnerability == null)
            return Result<GetRiskDetailResponse>.Failure("Vulnerabilidade não encontrada.");

        var combinationExists = await RiskCombinationExistsAsync(asset.Id, threat.Id, vulnerability.Id);
        if (combinationExists)
            return Result<GetRiskDetailResponse>.Failure("Já existe um risco com a combinação de ativo, ameaça e vulnerabilidade.");

        var risk = new Risk(request.AssetId, request.ThreatId, request.VulnerabilityId, request.Probability, request.Impact);

        await _dbContext.Risks.AddAsync(risk);
        await _dbContext.SaveChangesAsync();

        return await GetByIdAsync(risk.Id);
    }

    public async Task<Result<GetRiskDetailResponse>> UpdateAsync(Guid id, UpdateRiskRequest request)
    {
        var risk = await _dbContext.Risks.FindAsync(id);
        if (risk == null)
            return Result<GetRiskDetailResponse>.Failure("Risco não encontrado.");

        risk.ChangeStatus(request.Status);
        risk.ChangeProbability(request.Probability);
        risk.ChangeImpact(request.Impact);

        var resultTreatment = await ChangeTreatmentAsync(risk.Id, request.Treatment, request.TreatmentDescription);
        if (!resultTreatment.Succeeded)
            return Result<GetRiskDetailResponse>.Failure(resultTreatment.Errors);

        await _dbContext.SaveChangesAsync();

        return await GetByIdAsync(id);
    }

    public async Task<Result> DeleteAsync(Guid id)
    {
        var risk = await _dbContext.Risks.FindAsync(id);
        if (risk == null)
            return Result.Failure("Risco não encontrado.");

        _dbContext.Risks.Remove(risk);
        await _dbContext.SaveChangesAsync();

        return Result.Success();
    }

    public async Task<Result> AddControlAsync(Guid riskId, InsertRiskControlRequest request)
    {
        var risk = await _dbContext.Risks.Include(r => r.RiskControls).FirstOrDefaultAsync(r => r.Id == riskId);
        if (risk == null)
            return Result.Failure("Risco não encontrado.");

        var control = await _dbContext.Controls.FindAsync(request.ControlId);
        if (control == null)
            return Result.Failure("Controle não encontrado.");

        risk.AddControl(control.Id, control.Type, request.Effectiveness);

        await _dbContext.SaveChangesAsync();

        return Result.Success();
    }

    public async Task<Result> RemoveControlAsync(Guid riskId, Guid controlId)
    {
        var risk = await _dbContext.Risks.Include(r => r.RiskControls).FirstOrDefaultAsync(r => r.Id == riskId);
        if (risk == null)
            return Result.Failure("Risco não encontrado.");

        var control = await _dbContext.Controls.FindAsync(controlId);
        if (control == null)
            return Result.Failure("Controle não encontrado.");

        risk.RemoveControl(control.Id);

        await _dbContext.SaveChangesAsync();

        return Result.Success();
    }

    public async Task<Result> ChangeControlEffectivenessAsync(Guid riskId, UpdateRiskControlEffectivenessRequest request)
    {
        var risk = await _dbContext.Risks
            .Include(r => r.RiskControls)
            .FirstOrDefaultAsync(r => r.Id == riskId);

        if (risk == null)
            return Result.Failure("Risco não encontrado");

        var control = risk.RiskControls.FirstOrDefault(rc => rc.ControlId == request.ControlId);
        if (control == null)
            return Result.Failure("Controle não encontrado");

        control.ChangeEffectiveness(request.Effectiveness);
        risk.RecalculateEffectiveness();

        await _dbContext.SaveChangesAsync();

        return Result.Success();
    }

    public async Task<Result<List<GetRiskControlResponse>>> GetListControlsAsync(Guid riskId)
    {
        var riskControls = await _dbContext.RiskControls
            .AsNoTracking()
            .Where(rc => rc.RiskId == riskId)
            .ProjectTo<GetRiskControlResponse>(_mapper.ConfigurationProvider)
            .ToListAsync();

        return Result<List<GetRiskControlResponse>>.Success(riskControls);
    }

}
