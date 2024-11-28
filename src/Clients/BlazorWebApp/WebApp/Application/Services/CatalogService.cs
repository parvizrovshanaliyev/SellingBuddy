using System.Net.Http;
using System.Threading.Tasks;
using WebApp.Application.Services.Catalog;
using WebApp.Domain;
using WebApp.Domain.Catalog;
using WebApp.Extensions;

namespace WebApp.Application.Services;

public class CatalogService : ICatalogService
{
    private readonly HttpClient _httpClient;

    public CatalogService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<PaginatedItemsViewModel<CatalogItem>> GetCatalogItems(int pageIndex, int pageSize)
    {
        var uri = $"/catalog/items?pageIndex={pageIndex}&pageSize={pageSize}";
        var response = await _httpClient.GetResponseAsync<PaginatedItemsViewModel<CatalogItem>>(uri);
        return response;
    }
}