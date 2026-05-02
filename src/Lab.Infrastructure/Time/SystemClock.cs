using Lab.Domain.Common;

namespace Lab.Infrastructure.Time;

public class SystemClock : ISystemClock
{
    public DateTime UtcNow => DateTime.UtcNow;
}
