using System;
using System.Net.Http;
using System.Security.Principal;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using WebApp.Application.Services.Identity;
using WebApp.Application.Services.Interfaces;
using WebApp.Domain.Basket;
using WebApp.Domain.Catalog;
using WebApp.Extensions;
using WebApp.Infrastructure.HttpClient;

namespace WebApp.Application.Services;

public class BasketService : IBasketService
{
    private readonly HttpClient _client;
    private readonly IIdentityService _identityService;
    private readonly ILogger<BasketService> _logger;

    public BasketService(HttpClient client, IIdentityService identityService, ILogger<BasketService> logger)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _identityService = identityService ?? throw new ArgumentNullException(nameof(identityService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<Basket> GetBasketAsync()
    {
        var userName = _identityService.GetUserName();
        
        try
        {
            var url = $"/basket/{_identityService.GetUserName()}";
            var response = await _client.GetResponseAsync<Basket>(url);
            return response ?? new Basket() { BuyerId = userName };
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Error fetching basket for user {UserName}", userName);
            return new Basket { BuyerId = userName }; // Return an empty basket if the request fails
        }
    }

    public async Task<Basket> UpdateBasketAsync(Basket basket)
    {
        if (basket == null) throw new ArgumentNullException(nameof(basket));
        
        try
        {
            var url = $"/basket/update";
            var response = await _client.PostGetResponseAsync<Basket, Basket>(url, basket);
            return response;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Error updating basket for user {UserId}", basket.BuyerId);
            throw new InvalidOperationException("Failed to update the basket.", ex);
        }
    }

    public async Task AddItemToBasketAsync(int productId)
    {
        var url = $"/basket/items";

        var request = new
        {
            CatalogItemId = productId,
            Quantity = 1,
            BasketId = _identityService.GetUserName()
        };
        
        var response = await _client.PostAsync(url, request);
    }

    public async Task<bool> CheckoutAsync(BasketDto basket)
    {
        if (basket == null) throw new ArgumentNullException(nameof(basket));
        
        try
        {
            var url = $"/basket/checkout";
            var response = await _client.PostAsync(url, basket);
            return response.Success;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Error during checkout for basket {BasketId}", basket.Buyer);
            return false;
        }
    }
}