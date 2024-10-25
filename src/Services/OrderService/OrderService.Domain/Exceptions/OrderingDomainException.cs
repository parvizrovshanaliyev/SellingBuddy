namespace OrderService.Domain.Exceptions;

/// <summary>
/// Represents an exception specific to the ordering domain, used to indicate 
/// business rule violations or unexpected issues within the ordering process.
/// Inherits from <see cref="Exception"/>.
/// </summary>
public class OrderingDomainException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="OrderingDomainException"/> class.
    /// </summary>
    public OrderingDomainException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="OrderingDomainException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    public OrderingDomainException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="OrderingDomainException"/> class with a specified error message
    /// and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public OrderingDomainException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
