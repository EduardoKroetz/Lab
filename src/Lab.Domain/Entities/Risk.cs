using Lab.Domain.Common;
using Lab.Domain.Enums;
using Lab.Domain.Exceptions;

namespace Lab.Domain.Entities;

public class Risk : TenantEntity
{
    private readonly ISystemClock _clock;

    protected Risk() { } // EF
    public Risk(Guid assetId, Guid threatId, Guid vulnerabilityId, int probability, int impact, DateTime? reviewFixedDate, TimeSpan? reviewInterval, ISystemClock clock)
    {
        Status = ERiskStatus.Identified;
        AssetId = assetId;
        ThreatId = threatId;
        VulnerabilityId = vulnerabilityId;
        _clock = clock;

        SetProbability(probability);
        SetImpact(impact);
        SetReviewSchedule(reviewFixedDate, reviewInterval, clock);
    }

    public Guid AssetId { get; private set; }
    public Guid ThreatId { get; private set; }
    public Guid VulnerabilityId { get; private set; }

    public int Probability { get; private set; }
    public int Impact { get; private set; }

    public ERiskStatus Status { get; private set; }
    public string? ReasonForClose { get; private set; }

    public ERiskTreatment? Treatment { get; private set; }
    public string? TreatmentDescription { get; private set; }

    public double EffectivenessOnProbability { get; private set; }
    public double EffectivenessOnImpact { get; private set; }

    public int RawScore => Probability * Impact;
    public double ResidualScore => CalculateResidualScore(Probability, Impact, EffectivenessOnProbability, EffectivenessOnImpact);

    public ERiskLevel Level => ResidualScore switch
    {
        >= 20 => ERiskLevel.Critical,
        >= 15 => ERiskLevel.High,
        >= 10 => ERiskLevel.Medium,
        _ => ERiskLevel.Low
    };

    public DateTime? ReviewFixedDate { get; private set; }
    public TimeSpan? ReviewInterval { get; private set; }
    public DateTime? LastEvaluatedAt { get; private set; }
    public DateTime? NextReviewDate
    {
        get
        {
            if (ReviewFixedDate.HasValue)
                return ReviewFixedDate;

            if (!ReviewInterval.HasValue)
                return null;

            return (LastEvaluatedAt ?? _clock.UtcNow).Add(ReviewInterval.Value);
        }
    }

    #region Navigation Properties

    public Asset Asset { get; private set; } = null!;
    public Threat Threat { get; private set; } = null!;
    public Vulnerability Vulnerability { get; private set; } = null!;

    #endregion

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

    public void SetProbability(int probability)
    {
        if (probability <= 0 || probability > 5)
            throw new DomainException("A probabilidade deve estar entre 0 e 5");

        Probability = probability;
    }

    public void SetImpact(int impact)
    {
        if (impact <= 0 || impact > 5)
            throw new DomainException("O impacto deve estar entre 0 e 5");

        Impact = impact;
    }

    public void SetReviewSchedule(DateTime? reviewFixedDate, TimeSpan? reviewInterval, ISystemClock clock)
    {
        const int MaxReviewYears = 10;

        var now = clock.UtcNow;
        var maxDate = now.AddYears(MaxReviewYears);

        if (reviewFixedDate is null && reviewInterval is null)
            throw new DomainException("Revisões de risco são obrigatórias: informe uma data fixa ou um intervalo.");

        if (reviewFixedDate is not null && reviewInterval is not null)
            throw new DomainException("Só é possível especificar um tipo de revisão: data fixa ou intervalo.");

        if (reviewFixedDate is not null)
        {
            if (reviewFixedDate <= now)
                throw new DomainException("A data de revisão deve estar no futuro.");

            if (reviewFixedDate > maxDate)
                throw new DomainException($"A data de revisão não pode ser maior que {MaxReviewYears} anos.");
        }

        if (reviewInterval is not null)
        {
            if (reviewInterval <= TimeSpan.Zero)
                throw new DomainException("O intervalo de revisão deve ser maior que zero.");

            if (reviewInterval > TimeSpan.FromDays(365 * MaxReviewYears))
                throw new DomainException($"O intervalo não pode ser maior que {MaxReviewYears} anos.");
        }

        ReviewFixedDate = reviewFixedDate;
        ReviewInterval = reviewInterval;
    }

    public void MarkAsEvaluated()
    {
        LastEvaluatedAt = _clock.UtcNow;
    }

    private readonly List<Incident> _incidents = [];
    public IReadOnlyCollection<Incident> Incidents => _incidents.AsReadOnly();

    public void AddIncident(Incident newIncident)
    {
        if (newIncident.RiskId != Id)
            throw new DomainException("Não é possível adicionar um incidente não relacionada ao risco.");

        _incidents.Add(newIncident);

        StartTreatment();
    }

    private readonly List<WorkItem> _workItems = [];
    public IReadOnlyCollection<WorkItem> WorkItems => _workItems.AsReadOnly();
    public void AddWorkItem(WorkItem workItem)
    {
        if (workItem.RelatedRiskId != Id)
            throw new DomainException("Não é possível adicionar uma tarefa não relacionada ao risco.");

        _workItems.Add(workItem);

        if (workItem.IsActionTask && Status != ERiskStatus.UnderTreatment)
            StartTreatment();
    }

    public void StartTreatment()
    {
        var hasOpenIncident = _incidents.Any(i => i.Status is EIncidentStatus.Open);
        var hasOpenActionTask = _workItems.Any(t => t.IsOpen && t.IsActionTask);

        if (!hasOpenActionTask && !hasOpenIncident)
            throw new DomainException("É necessário ao menos uma tarefa de ação aberta ou que haja um incidente aberto.");

        Status = ERiskStatus.UnderTreatment;
    }

    public void EnterMonitoring()
    {
        var hasOpenTask = _workItems.Any(t => t.IsOpen);
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

    private readonly List<RiskHistory> _histories = [];
    public IReadOnlyCollection<RiskHistory> Histories => _histories.AsReadOnly();
    public void AddHistory(ERiskHistoryEvent historyEvent, ISystemClock clock)
    {
        var snapshot = new RiskSnapshot
        (
            Id,
            AssetId,
            ThreatId,
            VulnerabilityId,
            Probability,
            Impact,
            Status,
            Treatment,
            TreatmentDescription,
            RawScore,
            ResidualScore,
            Level,
            EffectivenessOnProbability,
            EffectivenessOnImpact,
            ReviewFixedDate,
            ReviewInterval,
            LastEvaluatedAt,
            NextReviewDate
        );

        var newHistory = new RiskHistory(Id, historyEvent, snapshot, clock);
        _histories.Add(newHistory);
    }
}
