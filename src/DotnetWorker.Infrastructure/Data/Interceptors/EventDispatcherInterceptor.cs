using Ardalis.SharedKernel;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace DotnetWorker.Infrastructure.Data.Interceptors;

// Interceptor to dispatch domain events after saving changes
public sealed class EventDispatcherInterceptor(IDomainEventDispatcher domainEventDispatcher)
    : SaveChangesInterceptor
{
    public override async ValueTask<int> SavedChangesAsync(
        SaveChangesCompletedEventData eventData,
        int result,
        CancellationToken cancellationToken = default)
    {
        var context = eventData.Context;
        if (context is not AppDbContext appDbContext)
        {
            return await base.SavedChangesAsync(eventData, result, cancellationToken).ConfigureAwait(false);
        }

        // Retrieve all tracked entities that have domain events
        var entitiesWithEvents = appDbContext.ChangeTracker.Entries<HasDomainEventsBase>()
          .Select(e => e.Entity)
          .Where(e => e.DomainEvents.Any())
          .ToArray();

        // Dispatch and clear domain events
        await domainEventDispatcher.DispatchAndClearEvents(entitiesWithEvents);

        return await base.SavedChangesAsync(eventData, result, cancellationToken);
    }
}
