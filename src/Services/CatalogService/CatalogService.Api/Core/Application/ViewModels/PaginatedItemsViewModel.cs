namespace CatalogService.Api.Core.Application.ViewModels;

public class PaginatedItemsViewModel<TEntity> where TEntity : class
{
    public PaginatedItemsViewModel(int pageIndex, int pageSize, long count, IEnumerable<TEntity> data)
    {
        PageIndex = pageIndex;
        PageSize = pageSize;
        Count = count;
        Data = data;
    }

    public int PageIndex { get; }
    public int PageSize { get; }
    public long Count { get; }
    
    public int TotalPages => (int)Math.Ceiling(Count / (double)PageSize);
    public IEnumerable<TEntity> Data { get; }
}