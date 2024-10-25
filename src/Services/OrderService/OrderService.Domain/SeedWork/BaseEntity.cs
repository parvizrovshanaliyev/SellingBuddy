using MediatR;

namespace OrderService.Domain.SeedWork;

/// <summary>
/// Represents the base entity for domain models, providing common properties and functionality
/// for domain event handling, equality checks, and unique identifier management.
/// </summary>
public abstract class BaseEntity
{
    /// <summary>
    /// Gets the unique identifier of the entity. Can be set only within the entity itself.
    /// </summary>
    public virtual Guid Id { get; protected set; }

    /// <summary>
    /// Gets or sets the date when the entity was created.
    /// </summary>
    public DateTime CreateDate { get; set; }

    private int? _requestedHashCode;
    private List<INotification> domainEvents;

    /// <summary>
    /// Gets a read-only collection of domain events associated with the entity.
    /// </summary>
    public IReadOnlyCollection<INotification> DomainEvents => domainEvents?.AsReadOnly();

    /// <summary>
    /// Adds a domain event to the entity's collection of domain events.
    /// </summary>
    /// <param name="eventItem">The domain event to add.</param>
    public void AddDomainEvent(INotification eventItem)
    {
        domainEvents ??= new List<INotification>();
        domainEvents.Add(eventItem);
    }

    /// <summary>
    /// Removes a domain event from the entity's collection of domain events.
    /// </summary>
    /// <param name="eventItem">The domain event to remove.</param>
    public void RemoveDomainEvent(INotification eventItem)
    {
        domainEvents?.Remove(eventItem);
    }

    /// <summary>
    /// Clears all domain events from the entity.
    /// </summary>
    public void ClearDomainEvents()
    {
        domainEvents?.Clear();
    }

    /// <summary>
    /// Determines if the entity is transient (i.e., has not been persisted with an assigned ID).
    /// </summary>
    /// <returns>True if the entity is transient; otherwise, false.</returns>
    public bool IsTransient()
    {
        return this.Id == default;
    }

    /// <summary>
    /// Determines whether the specified object is equal to the current entity based on its ID.
    /// </summary>
    /// <param name="obj">The object to compare with the current entity.</param>
    /// <returns>True if the specified object is equal to the current entity; otherwise, false.</returns>
    public override bool Equals(object obj)
    {
        if (obj == null || !(obj is BaseEntity))
        {
            return false;
        }

        if (ReferenceEquals(this, obj))
        {
            return true;
        }

        if (this.GetType() != obj.GetType())
        {
            return false;
        }

        BaseEntity item = (BaseEntity)obj;

        if (item.IsTransient() || this.IsTransient())
        {
            return false;
        }
        else
        {
            return item.Id == this.Id;
        }
    }

    /// <summary>
    /// Returns the hash code for this instance, using the entity's ID if it is not transient.
    /// </summary>
    /// <returns>The hash code for this instance.</returns>
    public override int GetHashCode()
    {
        if (!IsTransient())
        {
            if (!_requestedHashCode.HasValue)
            {
                _requestedHashCode = this.Id.GetHashCode() ^ 31;
            }

            return _requestedHashCode.Value;
        }
        else
        {
            return base.GetHashCode();
        }
    }

    /// <summary>
    /// Equality operator to determine if two entities are equal.
    /// </summary>
    public static bool operator ==(BaseEntity left, BaseEntity right)
    {
        if (Equals(left, null))
        {
            return Equals(right, null);
        }
        else
        {
            return left.Equals(right);
        }
    }

    /// <summary>
    /// Inequality operator to determine if two entities are not equal.
    /// </summary>
    public static bool operator !=(BaseEntity left, BaseEntity right)
    {
        return !(left == right);
    }
}
