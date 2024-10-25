namespace OrderService.Domain.AggregateModels.BuyerAggregate;

/// <summary>
/// Represents a type of card used for payment.
/// </summary>
public class CardType : Enumeration
{
    /// <summary>
    /// American Express card type.
    /// </summary>
    public static CardType Amex = new CardType(1, nameof(Amex).ToLowerInvariant());

    /// <summary>
    /// Visa card type.
    /// </summary>
    public static CardType Visa = new CardType(2, nameof(Visa).ToLowerInvariant());

    /// <summary>
    /// MasterCard card type.
    /// </summary>
    public static CardType MasterCard = new CardType(3, nameof(MasterCard).ToLowerInvariant());

    /// <summary>
    /// Initializes a new instance of the <see cref="CardType"/> class.
    /// </summary>
    /// <param name="id">The unique identifier for the card type.</param>
    /// <param name="name">The name of the card type.</param>
    public CardType(int id, string name) : base(id, name)
    {
    }
}