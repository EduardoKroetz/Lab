using Lab.Domain.Common;
using Lab.Domain.Enums;

namespace Lab.Domain.Entities;

public class RiskControl : TenantEntity
{
    protected RiskControl() { } // EF
    public RiskControl(Guid riskId, Guid controlId, EControlType controlType, int effectiveness)
    {
        RiskId = riskId;
        ControlId = controlId;
        ControlType = controlType;

        ChangeEffectiveness(effectiveness);
    }

    public Guid RiskId { get; private set; }
    public Guid ControlId { get; private set; }
    public int Effectiveness { get; private set; }
    public EControlType ControlType { get; private set; }

    public Control Control { get; set; } = null!;

    public void ChangeEffectiveness(int newEffectiveness)
    {
        if (newEffectiveness < 0 || newEffectiveness > 100)
            throw new InvalidOperationException("A eficácia deve estar entre 0 e 100");

        Effectiveness = newEffectiveness;
    }
}
