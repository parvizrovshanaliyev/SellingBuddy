namespace OrderService.Domain.Models;

/// <summary>
/// Represents a customer's shopping basket, containing the buyer's ID and a list of items in the basket.
/// </summary>
public class CustomerBasket
{
    /// <summary>
    /// Gets or sets the unique identifier of the buyer associated with this basket.
    /// </summary>
    public string BuyerId { get; set; }

    /// <summary>
    /// Gets or sets the list of items in the basket.
    /// </summary>
    public List<BasketItem> Items { get; set; } = new List<BasketItem>();

    /// <summary>
    /// Initializes a new instance of the <see cref="CustomerBasket"/> class with a specified buyer ID.
    /// </summary>
    /// <param name="customerId">The unique identifier of the buyer.</param>
    public CustomerBasket(string customerId)
    {
        BuyerId = customerId;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CustomerBasket"/> class.
    /// </summary>
    public CustomerBasket()
    {
    }
}
