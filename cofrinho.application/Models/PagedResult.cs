namespace cofrinho.application.Models;

public class PagedResult<T>
{
    public IEnumerable<T> Items { get; }
    public long TotalItems { get; }
    public int PageNumber { get; }
    public int PageSize { get; }
    public int TotalPages => (int)Math.Ceiling((double)TotalItems / PageSize);

    public PagedResult(IEnumerable<T> items, long totalItems, int pageNumber, int pageSize)
    {
        Items = items;
        TotalItems = totalItems;
        PageNumber = pageNumber;
        PageSize = pageSize;
    }

    public static PagedResult<T> Create(IQueryable<T> source, int pageNumber, int pageSize)
    {
        var totalItems = source.Count();
        var items = source
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return new PagedResult<T>(items, totalItems, pageNumber, pageSize);
    }
}