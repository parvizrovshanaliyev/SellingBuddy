using MediatR;
using OrderService.Domain.AggregateModels.BuyerAggregate;

namespace OrderService.Domain.Events;

/// <summary>
/// Event that represents verification of the buyer and payment method for an order.
/// Implements <see cref="INotification"/> for event handling.
/// </summary>
public class BuyerAndPaymentMethodVerifiedDomainEvent : INotification
{
    /// <summary>
    /// Gets or sets the buyer associated with the order.
    /// </summary>
    public Buyer Buyer { get; set; }

    /// <summary>
    /// Gets or sets the payment method used for the order.
    /// </summary>
    public PaymentMethod PaymentMethod { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier of the order.
    /// </summary>
    public Guid OrderId { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="BuyerAndPaymentMethodVerifiedDomainEvent"/> class 
    /// with specified buyer, payment method, and order ID.
    /// </summary>
    /// <param name="buyer">The buyer associated with the order.</param>
    /// <param name="paymentMethod">The payment method used for the order.</param>
    /// <param name="orderId">The unique identifier of the order.</param>
    public BuyerAndPaymentMethodVerifiedDomainEvent(Buyer buyer, PaymentMethod paymentMethod, Guid orderId)
    {
        Buyer = buyer;
        PaymentMethod = paymentMethod;
        OrderId = orderId;
    }
}