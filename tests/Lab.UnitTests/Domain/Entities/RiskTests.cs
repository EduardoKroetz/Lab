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
    // SetTreatment
    // -------------------------------------------------------

    [Fact]
    public void SetTreatment_MitigateWithoutDescription_MustNotThrow()
    {
        var risk = NewRisk();

        var exception = Record.Exception(() => risk.SetTreatment(ERiskTreatment.Mitigate, description: null));

        Assert.Null(exception);
    }

    [Theory]
    [InlineData(ERiskTreatment.Accept)]
    [InlineData(ERiskTreatment.Transfer)]
    [InlineData(ERiskTreatment.Eliminate)]
    public void SetTreatment_NonMitigateWithoutDescription_MustThrowDomainException(ERiskTreatment treatment)
    {
        var risk = NewRisk();

        Assert.Throws<DomainException>(() => risk.SetTreatment(treatment, description: null));
    }

    [Theory]
    [InlineData(ERiskTreatment.Accept)]
    [InlineData(ERiskTreatment.Transfer)]
    [InlineData(ERiskTreatment.Eliminate)]
    public void SetTreatment_NonMitigateWithDescription_MustSetTreatmentAndDescription(ERiskTreatment treatment)
    {
        var risk = NewRisk();

        risk.SetTreatment(treatment, description: "Justificativa.");

        Assert.Equal(treatment, risk.Treatment);
        Assert.Equal("Justificativa.", risk.TreatmentDescription);
    }

    [Fact]
    public void SetTreatment_MitigateWithDescription_MustSetBoth()
    {
        var risk = NewRisk();

        risk.SetTreatment(ERiskTreatment.Mitigate, description: "Observação.");

        Assert.Equal(ERiskTreatment.Mitigate, risk.Treatment);
        Assert.Equal("Observação.", risk.TreatmentDescription);
    }

    [Fact]
    public void SetTreatment_NonMitigateWithWhitespaceDescription_MustThrowDomainException()
    {
        var risk = NewRisk();

        Assert.Throws<DomainException>(() => risk.SetTreatment(ERiskTreatment.Accept, description: "   "));
    }


    // -------------------------------------------------------
    // Set Status
    // -------------------------------------------------------

    [Theory]
    [InlineData(ERiskStatus.Identified)]
    [InlineData(ERiskStatus.UnderTreatment)]
    [InlineData(ERiskStatus.Monitoring)]
    [InlineData(ERiskStatus.Closed)]
    public void SetStatus_AnyValidStatus_MustUpdateStatus(ERiskStatus status)
    {
        var risk = NewRisk();

        risk.SetStatus(status);

        Assert.Equal(status, risk.Status);
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
    public void RemoveControl_ControlNotInRisk_MustThrowDomainException()
    {
        var risk = NewRisk();

        Assert.Throws<DomainException>(() => risk.RemoveControl(Guid.NewGuid()));
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