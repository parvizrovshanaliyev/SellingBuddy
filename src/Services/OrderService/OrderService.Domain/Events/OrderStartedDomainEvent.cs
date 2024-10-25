using MediatR;
using OrderService.Domain.AggregateModels;
using OrderService.Domain.AggregateModels.OrderAggregate;

namespace OrderService.Domain.Events;

/// <summary>
/// Event that represents the initiation of an order. This domain event includes details 
/// of the user and payment card information used during order creation.
/// Implements <see cref="INotification"/> for event handling.
/// </summary>
public class OrderStartedDomainEvent : INotification
{
    /// <summary>
    /// Gets or sets the username of the person who started the order.
    /// </summary>
    public string UserName { get; set; }

    /// <summary>
    /// Gets or sets the type ID of the payment card used.
    /// </summary>
    public int CardTypeId { get; set; }

    /// <summary>
    /// Gets or sets the card number used for payment.
    /// </summary>
    public string CardNumber { get; set; }

    /// <summary>
    /// Gets or sets the security number (CVV) of the payment card.
    /// </summary>
    public string CardSecurityNumber { get; set; }  

    /// <summary>
    /// Gets or sets the name of the cardholder.
    /// </summary>
    public string CardHolderName { get; set; }

    /// <summary>
    /// Gets or sets the expiration date of the payment card.
    /// </summary>
    public DateTime CardExpiration { get; set; }

    /// <summary>
    /// Gets or sets the order associated with this event.
    /// </summary>
    public Order Order { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="OrderStartedDomainEvent"/> class 
    /// with specified order and card details.
    /// </summary>
    /// <param name="order">The order associated with the event.</param>
    /// <param name="userName">The username of the person who started the order.</param>
    /// <param name="cardTypeId">The type ID of the payment card used.</param>
    /// <param name="cardNumber">The card number used for payment.</param>
    /// <param name="cardSecurityNumber">The security number (CVV) of the card.</param>
    /// <param name="cardHolderName">The name of the cardholder.</param>
    /// <param name="cardExpiration">The expiration date of the card.</param>
    public OrderStartedDomainEvent(Order order,
        string userName,
        int cardTypeId,
        string cardNumber,
        string cardSecurityNumber,
        string cardHolderName,
        DateTime cardExpiration)
    {
        Order = order;
        UserName = userName;
        CardTypeId = cardTypeId;
        CardNumber = cardNumber;
        CardSecurityNumber = cardSecurityNumber;
        CardHolderName = cardHolderName;
        CardExpiration = cardExpiration;
    }
}