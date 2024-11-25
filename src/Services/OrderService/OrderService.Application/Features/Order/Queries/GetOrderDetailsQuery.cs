using System.Threading;
using System.Threading.Tasks;
using OrderService.Application.Interfaces.Repositories;

namespace OrderService.Application.Features.Order.Queries;

public class GetOrderDetailsQuery : IRequest<OrderDetailResponse>
{
    public Guid OrderId { get; set; }
    
    public GetOrderDetailsQuery(Guid orderId)
    {
        OrderId = orderId;
    }
}

public class GetOrderDetailsQueryHandler : IRequestHandler<GetOrderDetailsQuery, OrderDetailResponse>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IMapper _mapper;

    public GetOrderDetailsQueryHandler(IOrderRepository orderRepository, IMapper mapper)
    {
        _orderRepository = orderRepository;
        _mapper = mapper;
    }

    public async Task<OrderDetailResponse> Handle(GetOrderDetailsQuery request, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetByIdAsync(request.OrderId, cancellationToken, x => x.OrderItems);
        
        var orderDetailResponse = _mapper.Map<OrderDetailResponse>(order);
        
        return orderDetailResponse;
    }
}