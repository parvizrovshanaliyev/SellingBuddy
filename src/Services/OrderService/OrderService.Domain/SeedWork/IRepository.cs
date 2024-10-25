namespace OrderService.Domain.SeedWork;

/// <summary>
/// Represents a generic repository that provides access to the <see cref="IUnitOfWork"/> 
/// to manage transactions and consistency within the domain.
/// </summary>
public interface IRepository
{
    /// <summary>
    /// Gets the unit of work instance to handle transactions for this repository.
    /// </summary>
    IUnitOfWork UnitOfWork { get; }
}
