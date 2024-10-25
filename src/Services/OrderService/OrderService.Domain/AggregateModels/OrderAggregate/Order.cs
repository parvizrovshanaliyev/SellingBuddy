using OrderService.Domain.AggregateModels.BuyerAggregate;
using OrderService.Domain.Events;

namespace OrderService.Domain.AggregateModels.OrderAggregate;

/// <summary>
/// Represents an order entity, containing details of the buyer, shipping address, 
/// order items, payment information, and order status.
/// Implements <see cref="IAggregateRoot"/> and inherits from <see cref="BaseEntity"/>.
/// </summary>
public class Order : BaseEntity, IAggregateRoot
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Order"/> class with a new ID and an empty order items list.
    /// </summary>
    protected Order()
    {
        Id = Guid.NewGuid();
        _orderItems = new List<OrderItem>();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Order"/> class with specified buyer, shipping, and payment details.
    /// Sets the initial order status to "Submitted" and triggers the OrderStarted domain event.
    /// </summary>
    /// <param name="username">The username of the person who placed the order.</param>
    /// <param name="address">The shipping address for the order.</param>
    /// <param name="cardTypeId">The type ID of the payment card.</param>
    /// <param name="cardNumber">The number of the payment card.</param>
    /// <param name="cardSecurityNumber">The security number (CVV) of the payment card.</param>
    /// <param name="cardHolderName">The name of the cardholder.</param>
    /// <param name="cardExpiration">The expiration date of the payment card.</param>
    /// <param name="paymentMethodId">Optional. The ID of the payment method.</param>
    /// <param name="buyerId">Optional. The ID of the buyer.</param>
    public Order(
        string username,
        Address address,
        int cardTypeId,
        string cardNumber,
        string cardSecurityNumber,
        string cardHolderName,
        DateTime cardExpiration,
        Guid? paymentMethodId,
        Guid? buyerId = null)
        : this()
    {
        BuyerId = Guid.NewGuid();
        OrderDate = DateTime.UtcNow;
        Address = address;
        OrderStatus = OrderStatus.Submitted;

        AddOrderStartedDomainEvent(
            username: username,
            cardTypeId: cardTypeId,
            cardNumber: cardNumber,
            cardSecurityNumber: cardSecurityNumber,
            cardHolderName: cardHolderName,
            cardExpiration: cardExpiration);
    }

    /// <summary>
    /// Gets or sets the date when the order was placed.
    /// </summary>
    public DateTime OrderDate { get; set; }

    /// <summary>
    /// Gets the total quantity of items in the order.
    /// </summary>
    public int Quantity { get; private set; }

    /// <summary>
    /// Gets a description of the order.
    /// </summary>
    public string Description { get; private set; }

    /// <summary>
    /// Gets or sets the ID of the buyer associated with the order.
    /// </summary>
    public Guid? BuyerId { get; private set; }

    /// <summary>
    /// Gets the buyer information associated with the order.
    /// </summary>
    public Buyer Buyer { get; private set; }

    /// <summary>
    /// Gets the shipping address for the order.
    /// </summary>
    public Address Address { get; private set; }

    /// <summary>
    /// Gets the current status of the order.
    /// </summary>
    public OrderStatus OrderStatus { get; private set; }

    /// <summary>
    /// Gets a read-only collection of items in the order.
    /// </summary>
    private readonly List<OrderItem> _orderItems;
    public IReadOnlyCollection<OrderItem> OrderItems => _orderItems;

    /// <summary>
    /// Gets or sets the ID of the payment method used for the order.
    /// </summary>
    public Guid? PaymentMethodId { get; private set; }

    /// <summary>
    /// Triggers an OrderStarted domain event, indicating the order initiation process.
    /// </summary>
    /// <param name="username">The username of the person who placed the order.</param>
    /// <param name="cardTypeId">The type ID of the payment card.</param>
    /// <param name="cardNumber">The number of the payment card.</param>
    /// <param name="cardSecurityNumber">The security number (CVV) of the payment card.</param>
    /// <param name="cardHolderName">The name of the cardholder.</param>
    /// <param name="cardExpiration">The expiration date of the payment card.</param>
    private void AddOrderStartedDomainEvent(
        string username,
        int cardTypeId,
        string cardNumber,
        string cardSecurityNumber,
        string cardHolderName,
        DateTime cardExpiration)
    {
        var orderStartedDomainEvent = new OrderStartedDomainEvent(
            order: this,
            userName: username,
            cardTypeId: cardTypeId,
            cardNumber: cardNumber,
            cardSecurityNumber: cardSecurityNumber,
            cardHolderName: cardHolderName,
            cardExpiration: cardExpiration);

        AddDomainEvent(orderStartedDomainEvent);
    }

    /// <summary>
    /// Adds an item to the order.
    /// </summary>
    /// <param name="productId">The product ID of the item to add.</param>
    /// <param name="productName">The name of the product.</param>
    /// <param name="unitPrice">The unit price of the product.</param>
    /// <param name="pictureUrl">The URL of the product image.</param>
    /// <param name="units">The number of units to add. Defaults to 1.</param>
    public void AddOrderItem(
        int productId,
        string productName,
        decimal unitPrice,
        string pictureUrl,
        int units = 1)
    {
        var orderItem = new OrderItem(productId, productName, unitPrice, pictureUrl, units);
        _orderItems.Add(orderItem);
    }

    /// <summary>
    /// Sets the ID of the buyer associated with the order.
    /// </summary>
    /// <param name="buyerId">The ID of the buyer.</param>
    public void SetBuyerId(Guid buyerId)
    {
        BuyerId = buyerId;
    }

    /// <summary>
    /// Sets the payment method ID for the order.
    /// </summary>
    /// <param name="paymentMethodId">The ID of the payment method.</param>
    public void SetPaymentMethodId(Guid paymentMethodId)
    {
        PaymentMethodId = paymentMethodId;
    }
}
