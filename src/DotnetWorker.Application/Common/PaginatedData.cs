namespace DotnetWorker.Application.Common;

public class PaginatedData<T>
{
    public PaginatedData(IReadOnlyList<T> items, int totalCount, int pageSize, int pageNumber)
    {
        Items = items;
        TotalCount = totalCount;
        PageSize = pageSize;
        PageNumber = pageNumber;
    }

    public IReadOnlyList<T> Items { get; }

    public int TotalCount { get; }

    public int PageSize { get; }

    public int PageNumber { get; }
}
