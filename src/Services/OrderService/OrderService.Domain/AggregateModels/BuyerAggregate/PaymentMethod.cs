namespace OrderService.Domain.AggregateModels.BuyerAggregate;

/// <summary>
/// Represents a payment method entity that contains card information such as card number, 
/// security number, card holder's name, expiration date, and card type.
/// Inherits from <see cref="BaseEntity"/>.
/// </summary>
public class PaymentMethod : BaseEntity
{ 
    public int BuyerId { get; set; }
    /// <summary>
    /// Gets or sets an alias name for the payment method, used for identification purposes.
    /// </summary>
    public string Alias { get; set; }

    /// <summary>
    /// Gets or sets the card number associated with this payment method.
    /// </summary>
    public string CardNumber { get; set; }

    /// <summary>
    /// Gets or sets the security number (CVV) of the card.
    /// </summary>
    public string SecurityNumber { get; set; }

    /// <summary>
    /// Gets or sets the name of the cardholder.
    /// </summary>
    public string CardHolderName { get; set; }

    /// <summary>
    /// Gets or sets the expiration date of the card.
    /// </summary>
    public DateTime Expiration { get; set; }

    /// <summary>
    /// Gets or sets the ID representing the type of card (e.g., Visa, MasterCard).
    /// </summary>
    public int CardTypeId { get; set; }

    /// <summary>
    /// Gets or sets the card type entity that defines the type of card.
    /// </summary>
    public CardType CardType { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="PaymentMethod"/> class.
    /// </summary>
    public PaymentMethod()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PaymentMethod"/> class with specific details.
    /// Validates non-nullable fields and checks that the expiration date is in the future.
    /// </summary>
    /// <param name="cardTypeId">The ID of the card type.</param>
    /// <param name="alias">Alias name for the payment method.</param>
    /// <param name="cardNumber">The card number associated with this payment method.</param>
    /// <param name="securityNumber">The security number (CVV) of the card.</param>
    /// <param name="cardHolderName">The name of the cardholder.</param>
    /// <param name="expiration">The expiration date of the card.</param>
    /// <exception cref="OrderingDomainException">Thrown if any required field is null or empty, or if expiration is in the past.</exception>
    public PaymentMethod(int cardTypeId, string alias, string cardNumber, string securityNumber, string cardHolderName, DateTime expiration)
    {
        CardNumber = !string.IsNullOrWhiteSpace(cardNumber) ? cardNumber : throw new OrderingDomainException(nameof(cardNumber));
        SecurityNumber = !string.IsNullOrWhiteSpace(securityNumber) ? securityNumber : throw new OrderingDomainException(nameof(securityNumber));
        CardHolderName = !string.IsNullOrWhiteSpace(cardHolderName) ? cardHolderName : throw new OrderingDomainException(nameof(cardHolderName));

        if (expiration < DateTime.UtcNow)
        {
            throw new OrderingDomainException(nameof(expiration));
        }

        Alias = alias;
        Expiration = expiration;
        CardTypeId = cardTypeId;
    }

    /// <summary>
    /// Compares this payment method with the provided card details to check if they are equivalent.
    /// </summary>
    /// <param name="cardTypeId">The ID of the card type to compare.</param>
    /// <param name="cardNumber">The card number to compare.</param>
    /// <param name="expiration">The expiration date to compare.</param>
    /// <returns>True if the card type, number, and expiration match; otherwise, false.</returns>
    public bool IsEqualTo(int cardTypeId, string cardNumber, DateTime expiration)
    {
        return CardTypeId == cardTypeId && CardNumber == cardNumber && Expiration == expiration;
    }
}
