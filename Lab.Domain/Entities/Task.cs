using Lab.Domain.Common;
using Lab.Domain.Enums;
using Lab.Domain.Exceptions;

namespace Lab.Domain.Entities;

public class Task : TenantEntity
{
    public Task(ETaskType type, Guid? relatedRiskId, Guid? relatedControlId, Guid? relatedIncidentId, string description, Guid assignedToUserId, ETaskStatus status, DateTime startDate, DateTime targetDate, int priority)
    {
        RelatedRiskId = relatedRiskId;
        RelatedControlId = relatedControlId;
        RelatedIncidentId = relatedIncidentId;
        Description = description;
        AssignedToUserId = assignedToUserId;
        Status = status;
        StartDate = startDate;
        TargetDate = targetDate;
        Priority = priority;

        ChangeType(type);
    }

    public ETaskType Type { get; private set; }
    public Guid? RelatedRiskId { get; private set; }
    public Guid? RelatedControlId { get; private set; }
    public Guid? RelatedIncidentId { get; private set; }
    public string Description { get; private set; }
    public Guid AssignedToUserId { get; private set; }
    public ETaskStatus Status { get; private set; }
    public DateTime StartDate { get; private set; }
    public DateTime TargetDate { get; private set; }
    public int Priority { get; private set; }
    public DateTime? CompletedAt { get; set; }
    public Guid? CompletedBy { get; set; }
    public string? ResultDescription { get; set; }

    public bool IsOpen => Status is ETaskStatus.Pending or ETaskStatus.InProgress;
    public bool IsActionTask => Type is ETaskType.RiskTreatment or ETaskType.ControlExecution or ETaskType.IncidentResponse;

    private void ChangeType(ETaskType newType)
    {
        if (newType == ETaskType.RiskTreatment && RelatedRiskId is null)
            throw new DomainException("Tarefa do tipo 'Tratamento de Risco' deve possuir um risco vinculado.");

        if (newType == ETaskType.ControlExecution && (RelatedRiskId is null || RelatedControlId is null))
            throw new DomainException("Tarefa do tipo 'Execução de Controle' deve possuir um risco e um controle vinculado.");

        if (newType == ETaskType.IncidentResponse && RelatedIncidentId is null)
            throw new DomainException("Tarefa do tipo 'Resposta de Incidente' deve possuir um incidente vinculado.");

        Type = newType;
    }

    public void Complete()
    {

    }
}
