using Lab.Domain.Common;

namespace Lab.IntegrationTests.Common;

public class FakeClock : ISystemClock
{
    public DateTime UtcNow { get; }

    public FakeClock(DateTime utcNow)
    {
        UtcNow = utcNow;
    }
}