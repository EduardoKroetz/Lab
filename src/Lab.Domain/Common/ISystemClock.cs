namespace Lab.Domain.Common;

public interface ISystemClock
{
    DateTime UtcNow { get; }
}
