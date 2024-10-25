namespace OrderService.Domain.AggregateModels.OrderAggregate;

/// <summary>
/// Represents an address with details such as street, city, state, country, and zip code.
/// </summary>
public record Address
{
    /// <summary>
    /// Gets the street address.
    /// </summary>
    public string Street { get; init; }

    /// <summary>
    /// Gets the city of the address.
    /// </summary>
    public string City { get; init; }

    /// <summary>
    /// Gets the state or region of the address.
    /// </summary>
    public string State { get; init; }

    /// <summary>
    /// Gets the country of the address.
    /// </summary>
    public string Country { get; init; }

    /// <summary>
    /// Gets the zip or postal code of the address.
    /// </summary>
    public string ZipCode { get; init; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Address"/> record.
    /// </summary>
    public Address()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Address"/> record with specific details.
    /// </summary>
    /// <param name="street">The street address.</param>
    /// <param name="city">The city of the address.</param>
    /// <param name="state">The state or region of the address.</param>
    /// <param name="country">The country of the address.</param>
    /// <param name="zipCode">The zip or postal code of the address.</param>
    public Address(string street, string city, string state, string country, string zipCode)
    {
        Street = street;
        City = city;
        State = state;
        Country = country;
        ZipCode = zipCode;
    }
}
