using AutoMapper;
using AutoMapper.QueryableExtensions;
using Lab.Application.Common.Interfaces;
using Lab.Application.DTOs.RiskControls;
using Lab.Application.DTOs.Risks;
using Lab.Domain.Entities;
using Lab.Domain.Enums;
using Lab.Domain.Exceptions;
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
        return await _dbContext.Risks.AnyAsync(r =>
            r.Id != riskId &&
            r.AssetId == assetId &&
            r.ThreatId == threatId &&
            r.VulnerabilityId == vulnerabilityId);
    }

    private async Task ChangeTreatmentAsync(Guid riskId, ERiskTreatment treatment, string? description)
    {
        var risk = await _dbContext.Risks.Include(r => r.RiskControls).FirstOrDefaultAsync(r => r.Id == riskId) ?? throw new NotFoundException("Risco não encontrado");

        if (treatment == ERiskTreatment.Eliminate)
        {
            var asset = await _dbContext.Assets.FirstOrDefaultAsync(a => a.Id == risk.AssetId) ?? throw new NotFoundException("Ativo não encontrado");

            if (asset.Enabled)
                throw new ValidationException("Para Eliminar, o ativo vinculado deve estar desabilitado.");
        }

        switch (treatment)
        {
            case ERiskTreatment.Mitigate:
            risk.Mitigate();
            break;

            case ERiskTreatment.Accept:
            risk.Accept(reason: description);
            break;

            case ERiskTreatment.Transfer:
            risk.Transfer(description: description);
            break;

            case ERiskTreatment.Eliminate:
            risk.Eliminate(reason: description);
            break;

            default:
            throw new InvalidOperationException("Tratamento inválido");
        }

        await _dbContext.SaveChangesAsync();
    }

    public async Task<List<GetRiskListResponse>> GetListAsync()
    {
        return await _dbContext.Risks
            .AsNoTracking()
            .Include(x => x.RiskControls)
            .ProjectTo<GetRiskListResponse>(_mapper.ConfigurationProvider)
            .ToListAsync();
    }

    public async Task<GetRiskDetailResponse> GetByIdAsync(Guid id)
    {
        var risk = await _dbContext.Risks
            .AsNoTracking()
            .Include(x => x.RiskControls)
            .ProjectTo<GetRiskDetailResponse>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(risk => risk.Id == id);

        if (risk == null)
            throw new NotFoundException("Risco não encontrado.");

        return risk;
    }

    public async Task<GetRiskDetailResponse> CreateAsync(InsertRiskRequest request)
    {
        var asset = await _dbContext.Assets.FindAsync(request.AssetId) ?? throw new NotFoundException("Ativo não encontrado.");
        var threat = await _dbContext.Threats.FindAsync(request.ThreatId) ?? throw new NotFoundException("Ameaça não encontrada.");
        var vulnerability = await _dbContext.Vulnerabilities.FindAsync(request.VulnerabilityId) ?? throw new NotFoundException("Vulnerabilidade não encontrada.");

        var combinationExists = await RiskCombinationExistsAsync(asset.Id, threat.Id, vulnerability.Id);
        if (combinationExists)
            throw new ValidationException("Já existe um risco com a combinação de ativo, ameaça e vulnerabilidade.");

        var risk = new Risk(request.AssetId, request.ThreatId, request.VulnerabilityId, request.Probability, request.Impact);

        await _dbContext.Risks.AddAsync(risk);
        await _dbContext.SaveChangesAsync();

        return await GetByIdAsync(risk.Id);
    }

    public async Task<GetRiskDetailResponse> UpdateAsync(Guid id, UpdateRiskRequest request)
    {
        var risk = await _dbContext.Risks.FindAsync(id) ?? throw new NotFoundException("Risco não encontrado.");

        risk.SetProbability(request.Probability);
        risk.SetImpact(request.Impact);

        await ChangeTreatmentAsync(risk.Id, request.Treatment, request.TreatmentDescription);
        await _dbContext.SaveChangesAsync();

        return await GetByIdAsync(id);
    }

    public async Task DeleteAsync(Guid id)
    {
        var risk = await _dbContext.Risks.FindAsync(id) ?? throw new NotFoundException("Risco não encontrado.");

        _dbContext.Risks.Remove(risk);
        await _dbContext.SaveChangesAsync();
    }

    public async Task AddControlAsync(Guid riskId, InsertRiskControlRequest request)
    {
        var risk = await _dbContext.Risks.Include(r => r.RiskControls).FirstOrDefaultAsync(r => r.Id == riskId) ?? throw new NotFoundException("Risco não encontrado.");
        var control = await _dbContext.Controls.FindAsync(request.ControlId) ?? throw new NotFoundException("Controle não encontrado.");

        risk.AddControl(control.Id, control.Type);

        await _dbContext.SaveChangesAsync();
    }

    public async Task RemoveControlAsync(Guid riskId, Guid controlId)
    {
        var risk = await _dbContext.Risks.Include(r => r.RiskControls).FirstOrDefaultAsync(r => r.Id == riskId) ?? throw new NotFoundException("Risco não encontrado.");
        var control = await _dbContext.Controls.FindAsync(controlId) ?? throw new NotFoundException("Controle não encontrado.");

        risk.RemoveControl(control.Id);

        await _dbContext.SaveChangesAsync();
    }

    public async Task ChangeControlEffectivenessAsync(Guid riskId, UpdateRiskControlEffectivenessRequest request)
    {
        var risk = await _dbContext.Risks
            .Include(r => r.RiskControls)
            .FirstOrDefaultAsync(r => r.Id == riskId) ?? throw new NotFoundException("Risco não encontrado");

        var control = risk.RiskControls.FirstOrDefault(rc => rc.ControlId == request.ControlId) ?? throw new NotFoundException("Controle não encontrado");

        control.ChangeEffectiveness(request.Effectiveness);
        risk.RecalculateEffectiveness();

        await _dbContext.SaveChangesAsync();
    }

    public async Task<List<GetRiskControlResponse>> GetListControlsAsync(Guid riskId)
    {
        return await _dbContext.RiskControls
            .AsNoTracking()
            .Where(rc => rc.RiskId == riskId)
            .ProjectTo<GetRiskControlResponse>(_mapper.ConfigurationProvider)
            .ToListAsync();
    }
}
