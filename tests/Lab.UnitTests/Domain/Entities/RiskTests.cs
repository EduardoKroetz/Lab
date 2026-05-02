using Lab.Domain.Common;
using Lab.Domain.Entities;
using Lab.Domain.Enums;
using Lab.Domain.Exceptions;
using System.Collections;

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
    // Review
    // -------------------------------------------------------

    [Fact]
    public void SetReviewSchedule_BothReviewDateAndIntervalNull_MustThrowDomainException()
    {
        Assert.Throws<DomainException>(() => NewRiskWithSchedule(reviewFixedDate: null, reviewInterval: null));
    }

    [Fact]
    public void SetReviewSchedule_BothReviewDateAndIntervalProvided_MustThrowDomainException()
    {
        Assert.Throws<DomainException>(() => NewRiskWithSchedule(reviewFixedDate: DateTime.UtcNow, reviewInterval: TimeSpan.FromDays(30)));
    }

    [Fact]
    public void SetReviewSchedule_ValidFixedDate_MustSetReviewFixedDate()
    {
        var fixedDate = DateTime.UtcNow.AddMonths(3);
        var risk = NewRiskWithSchedule(reviewFixedDate: fixedDate);

        Assert.Equal(fixedDate, risk.ReviewFixedDate);
    }

    [Fact]
    public void SetReviewSchedule_ValidFixedDate_MustNotSetReviewInterval()
    {
        var risk = NewRiskWithSchedule(reviewFixedDate: DateTime.UtcNow.AddMonths(3));

        Assert.Null(risk.ReviewInterval);
    }

    [Fact]
    public void NextReviewDate_WithFixedDate_MustReturnFixedDate()
    {
        var fixedDate = DateTime.UtcNow.AddMonths(3);
        var risk = NewRiskWithSchedule(reviewFixedDate: fixedDate);

        Assert.Equal(fixedDate, risk.NextReviewDate);
    }

    [Fact]
    public void NextReviewDate_WithInterval_MustReturnInterval()
    {
        var interval = TimeSpan.FromDays(60);
        var now = DateTime.UtcNow;
        var risk = NewRiskWithSchedule(reviewInterval: interval, now: now);

        Assert.Equal(now.Add(interval), risk.NextReviewDate);
    }

    // Review Fixed Date
    public class ReviewInvalidFixedDateData : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            var now = DateTime.UtcNow;

            yield return [now.AddDays(-1)];
            yield return [now];
            yield return [now.AddYears(10).AddDays(1)];
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    [Theory]
    [ClassData(typeof(ReviewInvalidFixedDateData))]
    public void SetReviewSchedule_InvalidFixedDate_MustThrowDomainException(DateTime invalidFixedDate)
    {
        Assert.Throws<DomainException>(() => NewRiskWithSchedule(reviewFixedDate: invalidFixedDate, reviewInterval: null));
    }

    // Review Interval
    public class ReviewInvalidIntervalData : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            yield return [TimeSpan.Zero];
            yield return [TimeSpan.FromDays(365 * 10 + 1)];
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    [Theory]
    [ClassData(typeof(ReviewInvalidIntervalData))]
    public void SetReviewSchedule_InvalidInterval_MustThrowDomainException(TimeSpan invalidInterval)
    {
        Assert.Throws<DomainException>(() => NewRiskWithSchedule(reviewFixedDate: null, reviewInterval: invalidInterval));
    }

    // -------------------------------------------------------
    // LastEvaluated
    // -------------------------------------------------------
    [Fact]
    public void MarkAsEvaluated_MustReturnLastEvaluatedAt()
    {
        var now = DateTime.UtcNow;
        var risk = NewRiskWithSchedule(reviewFixedDate: DateTime.UtcNow.AddMonths(3), now: now);

        risk.MarkAsEvaluated();

        Assert.Equal(now, risk.LastEvaluatedAt);
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
        Assert.Equal("Encerrado após monitoramento.", risk.ReasonForClose);
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
        var clock = new FakeClock(DateTime.UtcNow);

        return new Risk(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), probability, impact, reviewFixedDate: DateTime.UtcNow.AddMonths(3), reviewInterval: null, clock);
    }

    private static Risk NewRiskWithSchedule(DateTime? reviewFixedDate = null, TimeSpan? reviewInterval = null, DateTime? now = null)
    {
        var clock = new FakeClock(now ?? DateTime.UtcNow);

        return new Risk(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), probability: 3, impact: 5, reviewFixedDate: reviewFixedDate, reviewInterval: reviewInterval, clock);
    }

    private class FakeClock : ISystemClock
    {
        public DateTime UtcNow { get; }

        public FakeClock(DateTime utcNow)
        {
            UtcNow = utcNow;
        }
    }
}