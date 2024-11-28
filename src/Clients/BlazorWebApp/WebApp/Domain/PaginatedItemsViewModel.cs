using System.Collections.Generic;

namespace WebApp.Domain;

public class PaginatedItemsViewModel<TEntity> where TEntity : class
{
    public PaginatedItemsViewModel(int pageIndex, int pageSize, long count, IEnumerable<TEntity> data) : this()
    {
        PageIndex = pageIndex;
        PageSize = pageSize;
        Count = count;
        Data = data;
    }

    public PaginatedItemsViewModel()
    {
    }

    public int PageIndex { get; set; }
    public int PageSize { get; set; }
    public long Count { get; set; }
    
    public int TotalPages { get; set; }
    public IEnumerable<TEntity> Data { get; set; }
}