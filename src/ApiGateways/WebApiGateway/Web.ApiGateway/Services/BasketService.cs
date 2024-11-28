using Web.ApiGateway.Infrastructure.HttpClient; 
using Web.ApiGateway.Services.Interfaces;
using Web.ApiGateway.Models.Basket;

namespace Web.ApiGateway.Services
{
    public class BasketService : IBasketService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<BasketService> _logger;

        public BasketService(IHttpClientFactory httpClientFactory, ILogger<BasketService> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        /// <summary>
        /// Retrieves a basket by its ID.
        /// </summary>
        /// <param name="id">The unique identifier of the basket.</param>
        /// <returns>A task representing the asynchronous operation with the basket object as the result.</returns>
        public async Task<Basket> GetBasketAsync(string id)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("basket"); // Assuming the HttpClient is configured with the "basket" name

                _logger.LogInformation("Sending request to fetch basket with ID: {Id}", id);

                // Using the GetResponseAsync extension method for GET request
                var basket = await client.GetResponseAsync<Basket>(id);

                if (basket == null)
                {
                    _logger.LogWarning("Basket with ID: {Id} not found.", id);
                }

                return basket ?? new Basket { BuyerId = id };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving basket with ID: {Id}", id);
                throw;
            }
        }

        /// <summary>
        /// Updates or creates a basket.
        /// </summary>
        /// <param name="basket">The basket object to be updated or created.</param>
        /// <returns>A task representing the asynchronous operation with the updated basket object as the result.</returns>
        public async Task<Basket> UpdateBasketAsync(Basket basket)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("basket"); // Assuming the HttpClient is configured with the "basket" name
                var url = $"/update";

                _logger.LogInformation("Sending request to update basket with ID: {Id}", basket.BuyerId);

                // Using the PostGetResponseAsync extension method for POST/PUT request
                var updatedBasket = await client.PostGetResponseAsync<Basket, Basket>(url, basket);

                if (updatedBasket == null)
                {
                    _logger.LogWarning("Failed to update basket with ID: {Id}.", basket.BuyerId);
                }

                return updatedBasket;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating basket with ID: {Id}", basket.BuyerId);
                throw;
            }
        }
    }
}
