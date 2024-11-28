using Web.ApiGateway.Infrastructure.HttpClient;
using Web.ApiGateway.Models.Catalog;
using Web.ApiGateway.Services.Interfaces;

namespace Web.ApiGateway.Services;

public class CatalogService : ICatalogService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration; 
    private readonly ILogger<CatalogService> _logger;

    public CatalogService(
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration,
        ILogger<CatalogService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
        _logger = logger;
    }


    /// <summary>
    /// Fetches a catalog item by its ID.
    /// </summary>
    /// <param name="id">The unique identifier of the catalog item.</param>
    /// <returns>The catalog item, or null if not found.</returns>
    public async Task<CatalogItem> GetCatalogItemAsync(int id)
    {
        try
        {
            // Validate input
            if (id <= 0)
            {
                _logger.LogWarning("Invalid ID provided: {Id}. ID must be greater than zero.", id);
                throw new ArgumentException("Invalid ID. ID must be greater than zero.", nameof(id));
            }

            var client = _httpClientFactory.CreateClient("catalog");
            var url = $"/items/{id}";

            _logger.LogInformation("Sending request to fetch catalog item with ID: {Id}", id);

            // Use the extension method to send GET request
            var catalogItem = await client.GetResponseAsync<CatalogItem>(url);

            if (catalogItem == null)
            {
                _logger.LogWarning("Catalog item with ID: {Id} not found.", id);
            }

            return catalogItem;
        }
        catch (ArgumentException ex)
        {
            // Log and rethrow expected argument exceptions
            _logger.LogError(ex, "Invalid input parameters.");
            throw;
        }
        catch (HttpRequestException ex)
        {
            // Handle HTTP request specific exceptions
            _logger.LogError(ex, "HTTP request failed.");
            throw;
        }
        catch (Exception ex)
        {
            // Handle unexpected errors
            _logger.LogError(ex, "Unexpected error occurred while fetching catalog item with ID: {Id}", id);
            throw new ApplicationException("An unexpected error occurred while fetching the catalog item.", ex);
        }
    }

    /// <summary>
    /// Fetches multiple catalog items by their IDs.
    /// </summary>
    /// <param name="ids">A collection of catalog item IDs.</param>
    /// <returns>A collection of catalog items.</returns>
    public async Task<IEnumerable<CatalogItem>> GetCatalogItemsAsync(IEnumerable<int> ids)
    {
        // Check for null or empty input
        if (ids == null || !ids.Any())
        {
            _logger.LogWarning("No IDs were provided for fetching catalog items.");
            return Enumerable.Empty<CatalogItem>();
        }

        try
        {
            var client = _httpClientFactory.CreateClient("catalog");
            var url = "/items"; // Assuming a batch endpoint for multiple items.

            _logger.LogInformation("Sending request to fetch multiple catalog items. IDs: {Ids}", string.Join(",", ids));

            // Use the extension method to send POST request with a list of IDs
            var catalogItems = await client.PostGetResponseAsync<IEnumerable<CatalogItem>, IEnumerable<int>>(url, ids);

            if (catalogItems == null || !catalogItems.Any())
            {
                _logger.LogWarning("No catalog items found for the given IDs.");
            }

            return catalogItems ?? Enumerable.Empty<CatalogItem>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error occurred while fetching multiple catalog items.");
            throw new ApplicationException("An unexpected error occurred while fetching the catalog items.", ex);
        }
    }
}