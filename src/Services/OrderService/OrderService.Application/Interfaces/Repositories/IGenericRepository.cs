using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace OrderService.Application.Interfaces.Repositories;

public interface IGenericRepository<T> : IRepository<T> where T : BaseEntity
{
    Task<List<T>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<List<T>> GetAsync(
        Expression<Func<T, bool>> predicate = null,
        Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
        CancellationToken cancellationToken = default,params Expression<Func<T, object>>[] includes);
    Task<List<T>> GetAsync(
        Expression<Func<T, bool>> predicate = null,
        CancellationToken cancellationToken = default,
        params Expression<Func<T, object>>[] includes);
    Task<T> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<T> GetByIdAsync(Guid id,CancellationToken cancellationToken , params Expression<Func<T, object>>[] includes);
    Task<T> GetSingleAsync(Expression<Func<T, bool>> predicate,CancellationToken cancellationToken,params Expression<Func<T, object>>[] includes);
    Task<T> AddAsync(T entity, CancellationToken cancellationToken = default);
    T Update(T entity);
}