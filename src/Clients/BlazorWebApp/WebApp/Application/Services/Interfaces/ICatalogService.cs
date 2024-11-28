using System.Threading.Tasks;
using WebApp.Domain;
using WebApp.Domain.Catalog;

namespace WebApp.Application.Services.Interfaces;

public interface ICatalogService
{
    Task<PaginatedItemsViewModel<CatalogItem>> GetCatalogItems(int pageIndex, int pageSize);
}