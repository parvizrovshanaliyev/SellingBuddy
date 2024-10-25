namespace OrderService.Domain.SeedWork;

/// <summary>
/// Represents a base class for value objects, which are immutable objects defined by their properties rather than identity.
/// Implements equality based on the values of the properties.
/// </summary>
public abstract class ValueObject
{
    /// <summary>
    /// Checks if two value objects are equal using their equality components.
    /// </summary>
    /// <param name="left">The first value object to compare.</param>
    /// <param name="right">The second value object to compare.</param>
    /// <returns>True if the objects are equal; otherwise, false.</returns>
    protected static bool EqualOperator(ValueObject left, ValueObject right)
    {
        if (ReferenceEquals(left, null) ^ ReferenceEquals(right, null))
        {
            return false;
        }

        return ReferenceEquals(left, null) || left.Equals(right);
    }

    /// <summary>
    /// Checks if two value objects are not equal.
    /// </summary>
    /// <param name="left">The first value object to compare.</param>
    /// <param name="right">The second value object to compare.</param>
    /// <returns>True if the objects are not equal; otherwise, false.</returns>
    protected static bool NotEqualOperator(ValueObject left, ValueObject right)
    {
        return !(EqualOperator(left, right));
    }

    /// <summary>
    /// Returns the properties used to determine equality for the value object.
    /// </summary>
    /// <returns>An enumerable collection of properties for equality comparison.</returns>
    protected abstract IEnumerable<object> GetEqualityComponents();

    /// <summary>
    /// Determines whether the specified object is equal to the current object based on its equality components.
    /// </summary>
    /// <param name="obj">The object to compare with the current object.</param>
    /// <returns>True if the specified object is equal to the current object; otherwise, false.</returns>
    public override bool Equals(object obj)
    {
        if (obj == null || obj.GetType() != GetType())
        {
            return false;
        }

        var other = (ValueObject)obj;
        
        return this.GetEqualityComponents().SequenceEqual(other.GetEqualityComponents());
    }

    /// <summary>
    /// Returns the hash code for the value object based on its equality components.
    /// </summary>
    public override int GetHashCode()
    {
        return GetEqualityComponents()
            .Select(x => x != null ? x.GetHashCode() : 0)
            .Aggregate((x, y) => x ^ y);
    }

    /// <summary>
    /// Creates a shallow copy of the value object.
    /// </summary>
    /// <returns>A shallow copy of the value object.</returns>
    public ValueObject GetCopy()
    {
        return this.MemberwiseClone() as ValueObject;
    }
}