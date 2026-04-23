using Lab.Domain.Common;

namespace Lab.Domain.Entities;

public class RiskControl : TenantEntity
{
    internal RiskControl() { } // EF
    public RiskControl(Risk risk, Control control, int effectiveness)
    {
        Risk = risk;
        Control = control;
        Effectiveness = effectiveness;

        Validate();
    }

    public Guid RiskId { get; set; }
    public Guid ControlId { get; set; }
    public int? Effectiveness { get; set; }

    public Risk Risk { get; set; }
    public Control Control { get; set; }

    public void Update(Risk risk, Control control, int effectiveness)
    {
        Risk = risk;
        Control = control;
        Effectiveness = effectiveness;

        Validate();
    }

    private void Validate()
    {
        if (Effectiveness is not null && Effectiveness < 0 && Effectiveness > 100)
            throw new InvalidOperationException("A eficácia deve estar entre 0 e 100");
    }
}
