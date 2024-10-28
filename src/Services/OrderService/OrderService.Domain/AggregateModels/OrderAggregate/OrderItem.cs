using System.ComponentModel.DataAnnotations;

namespace OrderService.Domain.AggregateModels.OrderAggregate;

/// <summary>
/// Represents an item within an order, including product details, unit price, quantity, and image URL.
/// Implements <see cref="IValidatableObject"/> for custom validation logic.
/// </summary>
public class OrderItem : BaseEntity, IValidatableObject
{
    public int OrderId { get; private set; }
    /// <summary>
    /// Gets the product ID associated with the order item.
    /// </summary>
    public int ProductId { get; private set; }

    /// <summary>
    /// Gets the name of the product.
    /// </summary>
    public string ProductName { get; private set; }

    /// <summary>
    /// Gets the unit price of the product.
    /// </summary>
    public decimal UnitPrice { get; private set; }

    /// <summary>
    /// Gets the quantity (units) of the product in the order.
    /// </summary>
    public int Units { get; private set; }

    /// <summary>
    /// Gets the URL of the product image.
    /// </summary>
    public string PictureUrl { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="OrderItem"/> class with specified product details.
    /// </summary>
    /// <param name="productId">The ID of the product.</param>
    /// <param name="productName">The name of the product.</param>
    /// <param name="unitPrice">The unit price of the product.</param>
    /// <param name="pictureUrl">The URL of the product image.</param>
    /// <param name="units">The quantity of the product in the order, defaulting to 1.</param>
    public OrderItem(int productId, string productName, decimal unitPrice, string pictureUrl, int units = 1)
    {
        ProductId = productId;
        ProductName = productName;
        UnitPrice = unitPrice;
        Units = units;
        PictureUrl = pictureUrl;
    }

    /// <summary>
    /// Validates the order item to ensure data integrity.
    /// Ensures the number of units is at least 1.
    /// </summary>
    /// <param name="validationContext">The context for validation.</param>
    /// <returns>A collection of validation results indicating validation errors.</returns>
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var validationResults = new List<ValidationResult>();

        if (Units < 1)
        {
            validationResults.Add(new ValidationResult("Invalid number of units", new[] { nameof(Units) }));
        }

        return validationResults;
    }
}
