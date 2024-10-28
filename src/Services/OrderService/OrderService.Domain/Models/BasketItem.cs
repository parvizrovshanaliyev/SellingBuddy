namespace OrderService.Domain.Models;

/// <summary>
/// Represents an item in a shopping basket, containing product details such as ID, name, 
/// unit price, previous price, quantity, and image URL.
/// </summary>
public class BasketItem
{
    /// <summary>
    /// Gets or sets the unique identifier of the basket item.
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier of the product.
    /// </summary>
    public int ProductId { get; set; }

    /// <summary>
    /// Gets or sets the name of the product.
    /// </summary>
    public string ProductName { get; set; }

    /// <summary>
    /// Gets or sets the current unit price of the product.
    /// </summary>
    public decimal UnitPrice { get; set; }

    /// <summary>
    /// Gets or sets the previous unit price of the product, useful for showing discounts.
    /// </summary>
    public decimal OldUnitPrice { get; set; }

    /// <summary>
    /// Gets or sets the quantity of the product in the basket.
    /// </summary>
    public int Quantity { get; set; }

    /// <summary>
    /// Gets or sets the URL of the product image.
    /// </summary>
    public string PictureUrl { get; set; }
}
