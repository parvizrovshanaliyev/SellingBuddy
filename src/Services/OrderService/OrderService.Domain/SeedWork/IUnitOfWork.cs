using System.Collections;
using System.Threading;
using System.Threading.Tasks;

namespace OrderService.Domain.SeedWork;

/// <summary>
/// Represents a unit of work that coordinates saving changes and ensures transaction integrity across multiple repositories.
/// </summary>
public interface IUnitOfWork
{
    /// <summary>
    /// Asynchronously saves changes to the data source.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The number of state entries written to the underlying database.</returns>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously saves changes and updates entities, ensuring business rules are maintained.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>True if the entities were saved successfully; otherwise, false.</returns>
    Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default);
}