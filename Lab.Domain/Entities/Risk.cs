using Lab.Domain.Common;
using Lab.Domain.Enums;
using Lab.Domain.Exceptions;

namespace Lab.Domain.Entities;

public class Risk : TenantEntity
{
    protected Risk() { } // EF
    public Risk(Guid assetId, Guid threatId, Guid vulnerabilityId, int probability, int impact)
    {
        Status = ERiskStatus.Identified;
        AssetId = assetId;
        ThreatId = threatId;
        VulnerabilityId = vulnerabilityId;

        ChangeProbability(probability);
        ChangeImpact(impact);
    }

    public Guid AssetId { get; private set; }
    public Guid ThreatId { get; private set; }
    public Guid VulnerabilityId { get; private set; }

    public int Probability { get; private set; }
    public int Impact { get; private set; }

    public ERiskStatus Status { get; private set; }

    public ERiskTreatment? Treatment { get; private set; }
    public string? TreatmentDescription { get; private set; }

    public string? ReasonForClose { get; private set; }

    public int RawScore => Probability * Impact;
    public double ResidualScore => CalculateResidualScore(Probability, Impact, EffectivenessOnProbability, EffectivenessOnImpact);

    public ERiskLevel Level => ResidualScore switch
    {
        >= 20 => ERiskLevel.Critical,
        >= 15 => ERiskLevel.High,
        >= 10 => ERiskLevel.Medium,
        _ => ERiskLevel.Low
    };

    public double EffectivenessOnProbability { get; private set; }
    public double EffectivenessOnImpact { get; private set; }

    public Asset Asset { get; private set; } = null!;
    public Threat Threat { get; private set; } = null!;
    public Vulnerability Vulnerability { get; private set; } = null!;

    private readonly List<Task> _tasks = [];
    public IReadOnlyCollection<Task> Tasks => _tasks.AsReadOnly();

    private readonly List<Incident> _incidents = [];
    public IReadOnlyCollection<Incident> Incidents => _incidents.AsReadOnly();

    private readonly List<RiskControl> _riskControls = [];
    public IReadOnlyCollection<RiskControl> RiskControls => _riskControls.AsReadOnly();

    public void AddControl(Guid controlId, EControlType controlType)
    {
        if (_riskControls.Any(rc => rc.ControlId == controlId))
            throw new DomainException("Controle já está vinculado ao risco");

        var riskControl = new RiskControl(Id, controlId, controlType, effectiveness: null);
        _riskControls.Add(riskControl);

        RecalculateEffectiveness();
    }

    public void RemoveControl(Guid controlId)
    {
        var riskControl = _riskControls.FirstOrDefault(rc => rc.ControlId == controlId)
            ?? throw new DomainException("Controle não encontrado nesse risco");

        _riskControls.Remove(riskControl);
        RecalculateEffectiveness();
    }

    public void ApplyControlExecution(Guid controlId, int effectiveness)
    {
        var riskControl = _riskControls.FirstOrDefault(rc => rc.ControlId == controlId)
            ?? throw new DomainException("Controle não encontrado nesse risco");

        riskControl.ChangeEffectiveness(effectiveness);

        RecalculateEffectiveness();
    }

    public void RecalculateEffectiveness()
    {
        var probabilityRiskControls = _riskControls.Where(r => r.ControlType is EControlType.Preventive).ToList();
        var impactRiskControls = _riskControls.Where(r => r.ControlType is EControlType.Detective or EControlType.Corrective).ToList();

        EffectivenessOnProbability = CalculateEffectiveness(probabilityRiskControls);
        EffectivenessOnImpact = CalculateEffectiveness(impactRiskControls);
    }

    private static double CalculateResidualScore(int probability, int impact, double effProbability, double effImpact)
    {
        var p = probability * (1 - effProbability / 100.0);
        var i = impact * (1 - effImpact / 100.0);
        return p * i;
    }

    private static double CalculateEffectiveness(List<RiskControl> controls)
    {
        if (!controls.Any())
            return 0;

        var combined = 1 - controls.Aggregate(1.0, (product, rc) => product * (1 - (rc.Effectiveness ?? 0) / 100.0));
        return combined * 100;
    }

    public void ChangeProbability(int probability)
    {
        if (probability <= 0 || probability > 5)
            throw new DomainException("A probabilidade deve estar entre 0 e 5");

        Probability = probability;
    }

    public void ChangeImpact(int impact)
    {
        if (impact <= 0 || impact > 5)
            throw new DomainException("O impacto deve estar entre 0 e 5");

        Impact = impact;
    }

    public void AddIncident(Incident newIncident)
    {
        if (newIncident.RiskId != Id)
            throw new DomainException("Não é possível adicionar um incidente não relacionada ao risco.");

        _incidents.Add(newIncident);

        StartTreatment();
    }

    public void AddTask(Task newTask)
    {
        if (newTask.RelatedRiskId != Id)
            throw new DomainException("Não é possível adicionar uma tarefa não relacionada ao risco.");

        _tasks.Add(newTask);

        if (newTask.IsActionTask && Status != ERiskStatus.UnderTreatment)
            StartTreatment();
    }

    public void StartTreatment()
    {
        var hasOpenIncident = _incidents.Any(i => i.Status is EIncidentStatus.Open);
        var hasOpenActionTask = _tasks.Any(t => t.IsOpen && t.IsActionTask);

        if (!hasOpenActionTask && !hasOpenIncident)
            throw new DomainException("É necessário ao menos uma tarefa de ação aberta ou que haja um incidente aberto.");

        Status = ERiskStatus.UnderTreatment;
    }

    public void EnterMonitoring()
    {
        var hasOpenTask = _tasks.Any(t => t.IsOpen);
        if (hasOpenTask)
            throw new DomainException("Não é possível iniciar o monitoramento do risco: é necessário que todas as tarefas vinculadas ao risco sejam concluídas.");

        Status = ERiskStatus.Monitoring;
    }

    public void Close(string reason)
    {
        if (string.IsNullOrWhiteSpace(reason))
            throw new DomainException("O motivo de fechamento do risco não foi informado.");

        var treatmentAcceptOrEliminate = Treatment is ERiskTreatment.Accept or ERiskTreatment.Eliminate;

        if (!treatmentAcceptOrEliminate)
            throw new DomainException("Não é possível fechar o risco se o tratamento não for 'Aceito' ou 'Eliminado' e não for decisão manual.");

        Status = ERiskStatus.Closed;
        ReasonForClose = reason;
    }

    public void CloseManually(string reason)
    {
        if (string.IsNullOrWhiteSpace(reason))
            throw new DomainException("O motivo de fechamento do risco não foi informado.");

        Status = ERiskStatus.Closed;
        ReasonForClose = reason;
    }

    public void Mitigate()
    {
        if (!_riskControls.Any())
            throw new DomainException("Para Mitigar, o risco deve possuir ao menos um controle vinculado.");

        Treatment = ERiskTreatment.Mitigate;
    }

    public void Accept(string? reason)
    {
        if (string.IsNullOrWhiteSpace(reason))
            throw new DomainException("O tratamento 'Aceito' requer justificativa.");

        Treatment = ERiskTreatment.Accept;
        TreatmentDescription = reason;

        EnterMonitoring();
    }

    public void Transfer(string? description)
    {
        if (string.IsNullOrWhiteSpace(description))
            throw new DomainException("O tratamento 'Transferido' requer descrição.");

        Treatment = ERiskTreatment.Transfer;
        TreatmentDescription = description;
    }

    public void Eliminate(string? reason)
    {
        if (string.IsNullOrWhiteSpace(reason))
            throw new DomainException("O tratamento 'Eliminado' requer justificativa.");

        Treatment = ERiskTreatment.Eliminate;
        TreatmentDescription = reason;

        Close(reason);
    }

}
