using Ardalis.SharedKernel;
using Ardalis.Specification.EntityFrameworkCore;

namespace DotnetWorker.Infrastructure.Data;

public sealed class EfCoreRepository<T>(AppDbContext dbContext) :
    RepositoryBase<T>(dbContext), IReadRepository<T>, IRepository<T>
    where T : class, IAggregateRoot
{
}
