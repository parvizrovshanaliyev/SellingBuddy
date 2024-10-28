using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using OrderService.Application.Interfaces.Repositories;
using OrderService.Domain.SeedWork;

namespace OrderService.Infrastructure.Repositories;

public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
{
    private readonly OrderDbContext _context;

    public GenericRepository(OrderDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public IUnitOfWork UnitOfWork { get; }
    
    public virtual Task<List<T>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return _context.Set<T>().ToListAsync(cancellationToken);
    }

    public virtual Task<List<T>> GetAsync(
        Expression<Func<T, bool>> predicate = null,
        Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
        CancellationToken cancellationToken = default,params Expression<Func<T, object>>[] includes)
    {
        IQueryable<T> query = _context.Set<T>();
        
        if (includes != null)
        {
            foreach (Expression<Func<T, object>> include in includes)
            {
                query = query.Include(include);
            }
        }
        
        if (predicate != null)
        {
            query = query.Where(predicate);
        }
        
        if (orderBy != null)
        {
            return orderBy(query).ToListAsync(cancellationToken);
        }
        
        return query.ToListAsync(cancellationToken);
    }

    public virtual Task<List<T>> GetAsync(Expression<Func<T, bool>> predicate = null, params Expression<Func<T, object>>[] includes)
    {
        throw new NotImplementedException();
    }

    public virtual Task<List<T>> GetAsync(
        Expression<Func<T, bool>> predicate = null,
        CancellationToken cancellationToken = default,
        params Expression<Func<T, object>>[] includes)
    {
        return GetAsync(predicate,orderBy:null,cancellationToken,includes);
    }

    public virtual async Task<T> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Set<T>().FindAsync(new object[] { id }, cancellationToken);
    }

    public virtual async Task<T> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken,
        params Expression<Func<T, object>>[] includes)
    {
        IQueryable<T> query = _context.Set<T>();

        if (includes != null)
        {
            foreach (Expression<Func<T, object>> include in includes)
            {
                query = query.Include(include);
            }
        }

        return await query.FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
    }

    public virtual async Task<T> GetSingleAsync(
        Expression<Func<T, bool>> predicate,
        CancellationToken cancellationToken,
        params Expression<Func<T, object>>[] includes)
    {
        IQueryable<T> query = _context.Set<T>();

        if (includes != null)
        {
            foreach (Expression<Func<T, object>> include in includes)
            {
                query = query.Include(include);
            }
        }

        return await query.Where(predicate).SingleOrDefaultAsync(cancellationToken);
    }

    public virtual async Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        await _context.Set<T>().AddAsync(entity, cancellationToken);
        return entity;
    }

    public virtual T Update(T entity)
    {
        _context.Set<T>().Update(entity);
        
        return entity;
    }
}