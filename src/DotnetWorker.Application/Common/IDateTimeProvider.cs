namespace DotnetWorker.Application.Common;

public interface IDateTimeProvider
{
    DateTime UtcNow { get; }
}
