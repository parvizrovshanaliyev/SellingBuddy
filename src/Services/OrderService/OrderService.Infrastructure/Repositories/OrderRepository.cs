using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using OrderService.Application.Interfaces.Repositories;

namespace OrderService.Infrastructure.Repositories;

public class OrderRepository : GenericRepository<Order>, IOrderRepository
{
    private readonly OrderDbContext _context;

    public OrderRepository(OrderDbContext context) : base(context)
    {
        _context = context;
    }

    public override async Task<Order> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken,
        params Expression<Func<Order, object>>[] includes)
    {
        var entity = await base.GetByIdAsync(id, cancellationToken, includes);

        if (entity == null)
        {
            entity = _context.Orders.Local.FirstOrDefault(x => x.Id == id);
        }

        return entity;
    }
}