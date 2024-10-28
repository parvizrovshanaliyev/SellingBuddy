using System.Threading;
using System.Threading.Tasks;
using OrderService.Application.Interfaces.Repositories;
using OrderService.Domain.Events;

namespace OrderService.Application.DomainEventHandlers;

public class UpdateOrderWhenBuyerAndPaymentVerifiedDomainEventHandler : INotificationHandler<BuyerAndPaymentMethodVerifiedDomainEvent>
{
    private readonly IOrderRepository _orderRepository;

    public UpdateOrderWhenBuyerAndPaymentVerifiedDomainEventHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
    }

    public async Task Handle(BuyerAndPaymentMethodVerifiedDomainEvent notification, CancellationToken cancellationToken)
    {
        var orderToUpdate = await _orderRepository.GetByIdAsync(
            notification.OrderId,
            cancellationToken: cancellationToken);
        
        orderToUpdate.SetBuyerId(notification.Buyer.Id);
        orderToUpdate.SetPaymentMethodId(notification.PaymentMethod.Id);
        
        // set methods so validate
    }
}