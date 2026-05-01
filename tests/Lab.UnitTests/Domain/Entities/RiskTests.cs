using Lab.Domain.Entities;
using Lab.Domain.Enums;
using Lab.Domain.Exceptions;

namespace Lab.UnitTests.Domain.Entities;

public class RiskTests
{
    // -------------------------------------------------------
    // Constructor
    // -------------------------------------------------------

    [Theory]
    [InlineData(0)]
    [InlineData(6)]
    [InlineData(-1)]
    public void Constructor_InvalidProbability_MustThrowDomainException(int probability)
    {
        Assert.Throws<DomainException>(() => NewRisk(probability: probability, impact: 3));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(6)]
    [InlineData(-1)]
    public void Constructor_InvalidImpact_MustThrowDomainException(int impact)
    {
        Assert.Throws<DomainException>(() => NewRisk(probability: 3, impact: impact));
    }

    [Fact]
    public void Constructor_ValidParameters_StatusMustBeIdentified()
    {
        var risk = NewRisk();

        Assert.Equal(ERiskStatus.Identified, risk.Status);
    }

    // -------------------------------------------------------
    // RawScore / ResidualScore / Level
    // -------------------------------------------------------

    [Fact]
    public void RawScore_MustBeProbabilityTimesImpact()
    {
        var risk = NewRisk(probability: 3, impact: 4);

        Assert.Equal(12, risk.RawScore);
    }

    [Fact]
    public void ResidualScore_WithNoControls_MustEqualRawScore()
    {
        var risk = NewRisk(probability: 3, impact: 4);

        Assert.Equal(12.0, risk.ResidualScore);
    }

    [Fact]
    public void ResidualScore_WithPreventiveControlAt50Percent_MustReduceProbabilityHalf()
    {
        // p = 4 * (1 - 0.5) = 2.0 | i = 4 * (1 - 0) = 4.0 | score = 8.0
        var risk = NewRisk(probability: 4, impact: 4);
        var controlId = Guid.NewGuid();

        risk.AddControl(controlId, EControlType.Preventive);
        risk.ApplyControlExecution(controlId, effectiveness: 50);

        Assert.Equal(8.0, risk.ResidualScore);
    }

    [Fact]
    public void ResidualScore_TwoPreventiveControlsAt50Percent_MustUseCombinedFormula()
    {
        // combined = 1 - (0.5 * 0.5) = 75% | p = 4 * 0.25 = 1.0 | i = 4 | score = 4.0
        var risk = NewRisk(probability: 4, impact: 4);
        var control1 = Guid.NewGuid();
        var control2 = Guid.NewGuid();

        risk.AddControl(control1, EControlType.Preventive);
        risk.AddControl(control2, EControlType.Preventive);
        risk.ApplyControlExecution(control1, effectiveness: 50);
        risk.ApplyControlExecution(control2, effectiveness: 50);

        Assert.Equal(4.0, risk.ResidualScore);
    }

    [Theory]
    [InlineData(4, 5, ERiskLevel.Critical)]  // score = 20
    [InlineData(3, 5, ERiskLevel.High)]      // score = 15
    [InlineData(2, 5, ERiskLevel.Medium)]    // score = 10
    [InlineData(2, 4, ERiskLevel.Low)]       // score = 8
    public void Level_MustReflectResidualScore(int probability, int impact, ERiskLevel expectedLevel)
    {
        var risk = NewRisk(probability: probability, impact: impact);

        Assert.Equal(expectedLevel, risk.Level);
    }

    // -------------------------------------------------------
    // AddControl
    // -------------------------------------------------------

    [Fact]
    public void AddControl_NewControl_MustBeContainedInList()
    {
        var risk = NewRisk();
        var controlId = Guid.NewGuid();

        risk.AddControl(controlId, EControlType.Preventive);

        Assert.Single(risk.RiskControls);
    }

    [Fact]
    public void AddControl_SameControlTwice_MustThrowDomainException()
    {
        var risk = NewRisk();
        var controlId = Guid.NewGuid();

        risk.AddControl(controlId, EControlType.Preventive);

        Assert.Throws<DomainException>(() => risk.AddControl(controlId, EControlType.Preventive));
    }

    [Fact]
    public void AddControl_WithoutApplyingEffectiveness_EffectivenessMustBeZero()
    {
        var risk = NewRisk();

        risk.AddControl(Guid.NewGuid(), EControlType.Preventive);

        Assert.Equal(0, risk.EffectivenessOnProbability);
    }

    // -------------------------------------------------------
    // RemoveControl
    // -------------------------------------------------------

    [Fact]
    public void RemoveControl_ExistingControl_MustRemoveFromList()
    {
        var risk = NewRisk();
        var controlId = Guid.NewGuid();
        risk.AddControl(controlId, EControlType.Preventive);

        risk.RemoveControl(controlId);

        Assert.Empty(risk.RiskControls);
    }

    [Fact]
    public void RemoveControl_ExistingControlWithEffectiveness_MustResetEffectiveness()
    {
        var risk = NewRisk();
        var controlId = Guid.NewGuid();
        risk.AddControl(controlId, EControlType.Preventive);
        risk.ApplyControlExecution(controlId, effectiveness: 80);

        risk.RemoveControl(controlId);

        Assert.Equal(0, risk.EffectivenessOnProbability);
    }

    [Fact]
    public void RemoveControl_ControlNotInRisk_MustThrowDomainException()
    {
        var risk = NewRisk();

        Assert.Throws<DomainException>(() => risk.RemoveControl(Guid.NewGuid()));
    }

    // -------------------------------------------------------
    // ApplyControlExecution
    // -------------------------------------------------------

    [Fact]
    public void ApplyControlExecution_PreventiveControl_MustOnlyAffectProbability()
    {
        var risk = NewRisk(probability: 4, impact: 4);
        var controlId = Guid.NewGuid();
        risk.AddControl(controlId, EControlType.Preventive);

        risk.ApplyControlExecution(controlId, effectiveness: 50);

        Assert.Equal(50.0, risk.EffectivenessOnProbability);
        Assert.Equal(0.0, risk.EffectivenessOnImpact);
    }

    [Fact]
    public void ApplyControlExecution_DetectiveControl_MustOnlyAffectImpact()
    {
        var risk = NewRisk();
        var controlId = Guid.NewGuid();
        risk.AddControl(controlId, EControlType.Detective);

        risk.ApplyControlExecution(controlId, effectiveness: 50);

        Assert.Equal(50.0, risk.EffectivenessOnImpact);
        Assert.Equal(0.0, risk.EffectivenessOnProbability);
    }

    [Fact]
    public void ApplyControlExecution_CorrectiveControl_MustOnlyAffectImpact()
    {
        var risk = NewRisk();
        var controlId = Guid.NewGuid();
        risk.AddControl(controlId, EControlType.Corrective);

        risk.ApplyControlExecution(controlId, effectiveness: 50);

        Assert.Equal(50.0, risk.EffectivenessOnImpact);
        Assert.Equal(0.0, risk.EffectivenessOnProbability);
    }

    [Fact]
    public void ApplyControlExecution_ControlNotInRisk_MustThrowDomainException()
    {
        var risk = NewRisk();

        Assert.Throws<DomainException>(() => risk.ApplyControlExecution(Guid.NewGuid(), effectiveness: 50));
    }

    // -------------------------------------------------------
    // Treatment
    // -------------------------------------------------------

    [Fact]
    public void Mitigate_WithoutControls_MustThrowDomainException()
    {
        var risk = NewRisk();

        Assert.Throws<DomainException>(() => risk.Mitigate());
    }

    [Fact]
    public void Mitigate_WithAtLeastOneControl_MustSetTreatmentToMitigate()
    {
        var risk = NewRisk();
        risk.AddControl(Guid.NewGuid(), EControlType.Preventive);

        risk.Mitigate();

        Assert.Equal(ERiskTreatment.Mitigate, risk.Treatment);
    }

    [Fact]
    public void Accept_WithReason_MustSetTreatmentAndEnterMonitoring()
    {
        var risk = NewRisk();

        risk.Accept("Risco aceito formalmente pelo comitê.");

        Assert.Equal(ERiskTreatment.Accept, risk.Treatment);
        Assert.Equal(ERiskStatus.Monitoring, risk.Status);
    }

    [Fact]
    public void Accept_WithoutReason_MustThrowDomainException()
    {
        var risk = NewRisk();

        Assert.Throws<DomainException>(() => risk.Accept(null));
    }

    [Fact]
    public void Transfer_WithDescription_MustSetTreatmentToTransfer()
    {
        var risk = NewRisk();

        risk.Transfer("Transferido para seguradora XYZ.");

        Assert.Equal(ERiskTreatment.Transfer, risk.Treatment);
    }

    [Fact]
    public void Transfer_WithoutDescription_MustThrowDomainException()
    {
        var risk = NewRisk();

        Assert.Throws<DomainException>(() => risk.Transfer(null));
    }

    [Fact]
    public void Eliminate_WithReason_MustCloseRiskDirectly()
    {
        var risk = NewRisk();

        risk.Eliminate("Vulnerabilidade removida com patch.");

        Assert.Equal(ERiskTreatment.Eliminate, risk.Treatment);
        Assert.Equal(ERiskStatus.Closed, risk.Status);
    }

    // -------------------------------------------------------
    // Close / CloseManually
    // -------------------------------------------------------

    [Fact]
    public void Close_WithAcceptTreatment_MustCloseSuccessfully()
    {
        var risk = NewRisk();
        risk.Accept("Aceito.");

        risk.Close("Encerrado após monitoramento.");

        Assert.Equal(ERiskStatus.Closed, risk.Status);
    }

    [Fact]
    public void Close_WithMitigateTreatment_MustThrowDomainException()
    {
        var risk = NewRisk();
        risk.AddControl(Guid.NewGuid(), EControlType.Preventive);
        risk.Mitigate();

        Assert.Throws<DomainException>(() => risk.Close("tentativa inválida"));
    }

    [Fact]
    public void Close_WithEmptyReason_MustThrowDomainException()
    {
        var risk = NewRisk();
        risk.Accept("Aceito.");

        Assert.Throws<DomainException>(() => risk.Close("   "));
    }

    [Fact]
    public void CloseManually_WithReason_MustCloseRegardlessOfTreatment()
    {
        var risk = NewRisk();

        risk.CloseManually("Fechamento administrativo.");

        Assert.Equal(ERiskStatus.Closed, risk.Status);
        Assert.Equal("Fechamento administrativo.", risk.ReasonForClose);
    }

    [Fact]
    public void CloseManually_WithoutReason_MustThrowDomainException()
    {
        var risk = NewRisk();

        Assert.Throws<DomainException>(() => risk.CloseManually(""));
    }

    // -------------------------------------------------------
    // Helpers
    // -------------------------------------------------------

    private static Risk NewRisk(int probability = 3, int impact = 5)
    {
        return new Risk(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), probability, impact);
    }
}