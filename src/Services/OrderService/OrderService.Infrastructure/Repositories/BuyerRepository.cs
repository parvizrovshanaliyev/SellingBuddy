using OrderService.Application.Interfaces.Repositories;

namespace OrderService.Infrastructure.Repositories;

public class BuyerRepository : GenericRepository<Buyer>, IBuyerRepository
{
    public BuyerRepository(OrderDbContext context) : base(context)
    {
    }
}