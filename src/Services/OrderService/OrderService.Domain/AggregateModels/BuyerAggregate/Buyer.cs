using OrderService.Domain.Events;

namespace OrderService.Domain.AggregateModels.BuyerAggregate;

/// <summary>
/// Represents a buyer in the order service domain.
/// </summary>
public class Buyer : BaseEntity, IAggregateRoot
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Buyer"/> class.
    /// </summary>
    public Buyer()
    {
        _paymentMethods = new List<PaymentMethod>();
    }
    
    /// <summary>
    /// Initializes a new instance of the <see cref="Buyer"/> class with a specified name.
    /// </summary>
    /// <param name="name">The name of the buyer.</param>
    /// <exception cref="ArgumentNullException">Thrown when the name is null.</exception>
    public Buyer(string name)
        : this()
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
    }
    
    /// <summary>
    /// Gets or sets the name of the buyer.
    /// </summary>
    public string Name { get; set; }
    
    private List<PaymentMethod> _paymentMethods;
    
    /// <summary>
    /// Gets the payment methods associated with the buyer.
    /// </summary>
    public IReadOnlyCollection<PaymentMethod> PaymentMethods => _paymentMethods;
    
    /// <summary>
    /// Verifies or adds a payment method for the buyer.
    /// </summary>
    /// <param name="cardTypeId">The card type ID.</param>
    /// <param name="alias">The alias for the payment method.</param>
    /// <param name="cardNumber">The card number.</param>
    /// <param name="securityNumber">The security number.</param>
    /// <param name="cardHolderName">The card holder name.</param>
    /// <param name="expiration">The expiration date of the card.</param>
    /// <param name="orderId">The order ID associated with the payment method.</param>
    /// <returns>The verified or newly added payment method.</returns>
    public PaymentMethod VerifyOrAddPaymentMethod(
        int cardTypeId,
        string alias,
        string cardNumber,
        string securityNumber,
        string cardHolderName,
        DateTime expiration,
        Guid orderId)
    {
        var existingPayment = _paymentMethods.SingleOrDefault(p => p.IsEqualTo(cardTypeId, cardNumber, expiration));
        
        if (existingPayment != null)
        {
            // raise event
            AddDomainEvent(new BuyerAndPaymentMethodVerifiedDomainEvent(this, existingPayment, orderId));
            
            return existingPayment;
        }
        
        var payment = new PaymentMethod(cardTypeId, alias, cardNumber, securityNumber, cardHolderName, expiration);
        
        _paymentMethods.Add(payment);
        
        // raise event
        AddDomainEvent(new BuyerAndPaymentMethodVerifiedDomainEvent(this, payment, orderId));
        
        return payment;
    }
    
    /// <summary>
    /// Determines whether the specified object is equal to the current object.
    /// </summary>
    /// <param name="obj">The object to compare with the current object.</param>
    /// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
    public override bool Equals(object obj)
    {
        return base.Equals(obj) || (obj is Buyer buyer && Id.Equals(buyer.Id) && Name == buyer.Name);
    }
}