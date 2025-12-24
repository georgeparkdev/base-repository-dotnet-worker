using DotnetWorker.Application.Common;

namespace DotnetWorker.Infrastructure.Common;

public sealed class DateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
}
