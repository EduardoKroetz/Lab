using Lab.Domain.Entities;
using Lab.Domain.Enums;
using Lab.Domain.Exceptions;

namespace Lab.UnitTests.Domain.Entities;

public class IncidentTests
{
    [Fact]
    public void Constructor_ValidParameters_MustSetProperties()
    {
        var description = "Server crashed";
        var date = DateTime.UtcNow.AddDays(-3);
        var riskId = Guid.NewGuid();

        var incident = new Incident(description, date, EIncidentStatus.Open, riskId);

        Assert.Equal(description, incident.Description);
        Assert.Equal(date, incident.DateOccurred);
        Assert.Equal(EIncidentStatus.Open, incident.Status);
        Assert.Equal(riskId, incident.RiskId);
    }

    [Fact]
    public void AddImpact_SingleImpact_MustUpdateScore()
    {
        var incident = NewIncident();
        incident.AddImpact(EIncidentImpactType.Financial, 5, "desc");

        Assert.Equal(5, incident.Score);
    }

    [Fact]
    public void AddImpact_SingleImpact_MustAddToCollection()
    {
        var incident = NewIncident();
        incident.AddImpact(EIncidentImpactType.Financial, 5, "desc");

        Assert.Single(incident.IncidentImpacts);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(11)]
    [InlineData(100)]
    public void AddImpact_InvalidSeverityScore_MustThrowDomainException(int score)
    {
        var incident = NewIncident();

        Assert.Throws<DomainException>(() => incident.AddImpact(EIncidentImpactType.Financial, score, null));
    }

    [Theory]
    [InlineData(1, EIncidentSeverityLevel.Low)]
    [InlineData(11, EIncidentSeverityLevel.Medium)]
    [InlineData(21, EIncidentSeverityLevel.High)]
    [InlineData(36, EIncidentSeverityLevel.Critical)]
    public void SeverityLevel_MustReflectScore(int score, EIncidentSeverityLevel expected)
    {
        var incident = NewIncidentWithScore(score);

        Assert.Equal(expected, incident.SeverityLevel);
    }

    [Fact]
    public void RemoveImpact_OneOfManyImpacts_MustRecalculateScoreCorrectly()
    {
        var incident = NewIncident();
        incident.AddImpact(EIncidentImpactType.Financial, 5, null);
        incident.AddImpact(EIncidentImpactType.Legal, 3, null);
        var idToRemove = incident.IncidentImpacts.First().Id;

        incident.RemoveImpact(idToRemove);

        Assert.Equal(3, incident.Score);
        Assert.Single(incident.IncidentImpacts);
    }

    [Fact]
    public void RemoveImpact_UnexistingImpact_MustThrowException()
    {
        var incident = NewIncident();

        Assert.Throws<DomainException>(() => incident.RemoveImpact(Guid.NewGuid()));
    }

    // Helpers

    private static Incident NewIncident()
    {
        return new Incident("desc", DateTime.UtcNow, EIncidentStatus.Open, Guid.NewGuid());
    }

    private static Incident NewIncidentWithScore(int score)
    {
        var incident = NewIncident();

        // score 1-10 por impacto, então divide em chamadas
        var calls = score / 10;
        var remainder = score % 10;

        for (var i = 0; i < calls; i++)
            incident.AddImpact(EIncidentImpactType.Financial, 10, null);

        if (remainder > 0)
            incident.AddImpact(EIncidentImpactType.Financial, remainder, null);

        return incident;
    }

}
