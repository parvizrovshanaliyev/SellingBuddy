using Microsoft.AspNetCore.Mvc;
using Web.ApiGateway.Models;
using Web.ApiGateway.Models.Basket;
using Web.ApiGateway.Services.Interfaces;
using System.Linq;
using Web.ApiGateway.Models.Catalog;

namespace Web.ApiGateway.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BasketController : ControllerBase
{
    private readonly ICatalogService _catalogService; 
    private readonly IBasketService _basketService;  

    public BasketController(ICatalogService catalogService, IBasketService basketService)
    {
        _catalogService = catalogService ?? throw new ArgumentNullException(nameof(catalogService));
        _basketService = basketService ?? throw new ArgumentNullException(nameof(basketService));
    }

    /// <summary>
    /// Adds an item to the basket.
    /// </summary>
    /// <param name="request">The request containing the basket item details.</param>
    /// <returns>A response indicating the result of the add operation.</returns>
    [HttpPost("items")]
    public async Task<IActionResult> AddItemToBasket([FromBody] AddBasketItemRequest request)
    {
        if (request == null)
        {
            return BadRequest("Invalid request. The request body is required.");
        }

        // Input validation
        if (request.Quantity <= 0)
        {
            return BadRequest("Quantity must be greater than zero.");
        }

        try
        {
            // Step 1: Get the catalog item by its ID
            var catalogItem = await _catalogService.GetCatalogItemAsync(request.CatalogItemId);
            if (catalogItem == null)
            {
                return NotFound($"Catalog item with ID {request.CatalogItemId} not found.");
            }

            // Step 2: Get the current basket by its ID
            var basket = await _basketService.GetBasketAsync(request.BasketId);
            if (basket == null)
            {
                return NotFound($"Basket with ID {request.BasketId} not found.");
            }

            // Step 3: Add or update the item in the basket
            var updatedBasket = AddOrUpdateBasketItem(basket, catalogItem, request.Quantity);

            // Step 4: Update the basket
            updatedBasket = await _basketService.UpdateBasketAsync(updatedBasket);

            return Ok(updatedBasket); // Return the updated basket after the item has been added
        }
        catch (Exception ex)
        {
            // Log the error and return a 500 internal server error
            // _logger.LogError(ex, "Error adding item to basket.");
            return StatusCode(500, "An error occurred while adding the item to the basket.");
        }
    }

    /// <summary>
    /// Adds a new item or updates the quantity of an existing item in the basket.
    /// </summary>
    /// <param name="basket">The current basket.</param>
    /// <param name="catalogItem">The catalog item to add or update.</param>
    /// <param name="quantity">The quantity of the item to add.</param>
    /// <returns>The updated basket with the added/updated item.</returns>
    private Basket AddOrUpdateBasketItem(Basket basket, CatalogItem catalogItem, int quantity)
    {
        // Check if the product already exists in the basket
        var existingItem = basket.Items.SingleOrDefault(x => x.ProductId == catalogItem.Id);
        
        if (existingItem != null)
        {
            // If the product exists, increase its quantity
            existingItem.Quantity += quantity;
        }
        else
        {
            // If the product does not exist, add a new item to the basket
            basket.Items.Add(new BasketItem
            {
                Id = Guid.NewGuid().ToString(),
                ProductId = catalogItem.Id,
                ProductName = catalogItem.Name,
                Quantity = quantity,
                UnitPrice = catalogItem.Price,
                PictureUrl = catalogItem.PictureUri
            });
        }

        return basket;
    }
}
