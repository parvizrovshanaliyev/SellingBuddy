using System.Reflection;

namespace OrderService.Domain.SeedWork;

/// <summary>
/// Serves as a base class for creating type-safe enumerations with properties for ID and name.
/// Provides methods for retrieving all instances, comparing values, and parsing from ID or name.
/// </summary>
public abstract class Enumeration : IComparable
{
    /// <summary>
    /// Gets the name of the enumeration instance.
    /// </summary>
    public string Name { get; private set; }

    /// <summary>
    /// Gets the unique identifier for the enumeration instance.
    /// </summary>
    public int Id { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Enumeration"/> class with a specified ID and name.
    /// </summary>
    /// <param name="id">The unique identifier for the enumeration instance.</param>
    /// <param name="name">The name of the enumeration instance.</param>
    protected Enumeration(int id, string name)
    {
        Id = id;
        Name = name;
    }

    /// <summary>
    /// Returns the name of the enumeration instance.
    /// </summary>
    public override string ToString() => Name;

    /// <summary>
    /// Retrieves all instances of a given enumeration type.
    /// </summary>
    /// <typeparam name="T">The type of the enumeration.</typeparam>
    /// <returns>An enumerable collection of all instances of the enumeration type.</returns>
    public static IEnumerable<T> GetAll<T>() where T : Enumeration
    {
        var fields = typeof(T).GetFields(BindingFlags.Public |
                                         BindingFlags.Static |
                                         BindingFlags.DeclaredOnly);
        return fields.Select(f => f.GetValue(null)).Cast<T>();
    }

    /// <summary>
    /// Determines whether the specified object is equal to the current enumeration instance based on ID.
    /// </summary>
    /// <param name="obj">The object to compare with the current instance.</param>
    /// <returns>True if the specified object is equal to the current instance; otherwise, false.</returns>
    public override bool Equals(object obj)
    {
        var otherValue = obj as Enumeration;

        if (otherValue == null)
        {
            return false;
        }

        var typeMatches = GetType().Equals(obj.GetType());
        var valueMatches = Id.Equals(otherValue.Id);

        return typeMatches && valueMatches;
    }

    /// <summary>
    /// Returns the hash code for this instance, based on the ID.
    /// </summary>
    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }

    /// <summary>
    /// Parses and retrieves an instance of the enumeration based on its ID.
    /// </summary>
    /// <typeparam name="T">The type of the enumeration.</typeparam>
    /// <param name="value">The ID value to parse.</param>
    /// <returns>The matching enumeration instance.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the ID does not match any instance.</exception>
    public static T FromValue<T>(int value) where T : Enumeration
    {
        var matchingItem = Parse<T, int>(value, "value", item => item.Id == value);
        return matchingItem;
    }

    /// <summary>
    /// Parses and retrieves an instance of the enumeration based on its name.
    /// </summary>
    /// <typeparam name="T">The type of the enumeration.</typeparam>
    /// <param name="displayName">The display name to parse.</param>
    /// <returns>The matching enumeration instance.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the name does not match any instance.</exception>
    public static T FromDisplayName<T>(string displayName) where T : Enumeration
    {
        var matchingItem = Parse<T, string>(displayName, "display name", item => item.Name == displayName);
        return matchingItem;
    }

    /// <summary>
    /// Finds an instance of the enumeration based on a predicate, throwing an exception if not found.
    /// </summary>
    /// <typeparam name="T">The type of the enumeration.</typeparam>
    /// <typeparam name="K">The type of the value to match.</typeparam>
    /// <param name="value">The value to match.</param>
    /// <param name="description">A description of the value type for error messages.</param>
    /// <param name="predicate">The predicate to match the enumeration instance.</param>
    /// <returns>The matching enumeration instance.</returns>
    /// <exception cref="InvalidOperationException">Thrown if no matching item is found.</exception>
    private static T Parse<T, K>(K value, string description, Func<T, bool> predicate) where T : Enumeration
    {
        var matchingItem = GetAll<T>().FirstOrDefault(predicate);

        if (matchingItem == null)
        {
            var message = $"'{value}' is not a valid {description} in {typeof(T)}";
            throw new InvalidOperationException(message);
        }

        return matchingItem;
    }

    /// <summary>
    /// Compares the current instance with another object of the same type by ID.
    /// </summary>
    /// <param name="other">An object to compare with this instance.</param>
    /// <returns>A value that indicates the relative order of the objects being compared.</returns>
    public int CompareTo(object other) => Id.CompareTo(((Enumeration)other).Id);
}
