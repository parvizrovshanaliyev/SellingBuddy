namespace OrderService.Domain.AggregateModels.OrderAggregate;

/// <summary>
/// Represents the various statuses an order can have throughout its lifecycle.
/// Inherits from <see cref="Enumeration"/> to allow for extensible enumerations.
/// </summary>
public class OrderStatus : Enumeration
{
    /// <summary>
    /// Indicates an order that has been submitted.
    /// </summary>
    public static OrderStatus Submitted = new OrderStatus(1, nameof(Submitted).ToLowerInvariant());

    /// <summary>
    /// Indicates an order awaiting validation.
    /// </summary>
    public static OrderStatus AwaitingValidation = new OrderStatus(2, nameof(AwaitingValidation).ToLowerInvariant());

    /// <summary>
    /// Indicates an order with confirmed stock.
    /// </summary>
    public static OrderStatus StockConfirmed = new OrderStatus(3, nameof(StockConfirmed).ToLowerInvariant());

    /// <summary>
    /// Indicates an order that has been paid.
    /// </summary>
    public static OrderStatus Paid = new OrderStatus(4, nameof(Paid).ToLowerInvariant());

    /// <summary>
    /// Indicates an order that has been shipped.
    /// </summary>
    public static OrderStatus Shipped = new OrderStatus(5, nameof(Shipped).ToLowerInvariant());

    /// <summary>
    /// Indicates an order that has been cancelled.
    /// </summary>
    public static OrderStatus Cancelled = new OrderStatus(6, nameof(Cancelled).ToLowerInvariant());

    /// <summary>
    /// Initializes a new instance of the <see cref="OrderStatus"/> class with a specific ID and name.
    /// </summary>
    /// <param name="id">The unique identifier for the order status.</param>
    /// <param name="name">The name of the order status.</param>
    public OrderStatus(int id, string name) : base(id, name)
    {
    }

    /// <summary>
    /// Gets a list of all predefined order statuses.
    /// </summary>
    /// <returns>An enumerable collection of <see cref="OrderStatus"/>.</returns>
    public static IEnumerable<OrderStatus> List() =>
        new[] { Submitted, AwaitingValidation, StockConfirmed, Paid, Shipped, Cancelled };

    /// <summary>
    /// Returns an <see cref="OrderStatus"/> instance based on a provided status name.
    /// </summary>
    /// <param name="name">The name of the order status.</param>
    /// <returns>The matching <see cref="OrderStatus"/> instance.</returns>
    /// <exception cref="OrderingDomainException">
    /// Thrown if the provided name does not match any defined order statuses.
    /// </exception>
    public static OrderStatus FromName(string name)
    {
        var status = List().SingleOrDefault(s => string.Equals(s.Name, name, System.StringComparison.CurrentCultureIgnoreCase));

        return status ?? throw new OrderingDomainException($"Possible values for OrderStatus: {string.Join(",", List().Select(s => s.Name))}");
    }

    /// <summary>
    /// Returns an <see cref="OrderStatus"/> instance based on a provided status ID.
    /// </summary>
    /// <param name="id">The unique identifier of the order status.</param>
    /// <returns>The matching <see cref="OrderStatus"/> instance.</returns>
    /// <exception cref="OrderingDomainException">
    /// Thrown if the provided ID does not match any defined order statuses.
    /// </exception>
    public static OrderStatus From(int id)
    {
        var status = List().SingleOrDefault(s => s.Id == id);

        return status ?? throw new OrderingDomainException($"Possible values for OrderStatus: {string.Join(",", List().Select(s => s.Name))}");
    }
}
