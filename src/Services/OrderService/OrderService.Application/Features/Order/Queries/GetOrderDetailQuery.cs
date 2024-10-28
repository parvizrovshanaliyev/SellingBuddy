using System.Threading;
using System.Threading.Tasks;
using OrderService.Application.Interfaces.Repositories;

namespace OrderService.Application.Features.Order.Queries;

public class GetOrderDetailQuery : IRequest<OrderDetailResponse>
{
    public Guid OrderId { get; set; }
    
    public GetOrderDetailQuery(Guid orderId)
    {
        OrderId = orderId;
    }
}

public class GetOrderDetailQueryHandler : IRequestHandler<GetOrderDetailQuery, OrderDetailResponse>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IMapper _mapper;

    public GetOrderDetailQueryHandler(IOrderRepository orderRepository, IMapper mapper)
    {
        _orderRepository = orderRepository;
        _mapper = mapper;
    }

    public async Task<OrderDetailResponse> Handle(GetOrderDetailQuery request, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetByIdAsync(request.OrderId, cancellationToken, x => x.OrderItems);
        
        var orderDetailResponse = _mapper.Map<OrderDetailResponse>(order);
        
        return orderDetailResponse;
    }
}