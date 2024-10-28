using System.Threading;
using System.Threading.Tasks;
using OrderService.Application.Interfaces.Repositories;
using OrderService.Domain.AggregateModels.BuyerAggregate;
using OrderService.Domain.Events;

namespace OrderService.Application.DomainEventHandlers;

public class OrderStartedDomainEventHandlers : INotificationHandler<OrderStartedDomainEvent>
{
    private readonly IBuyerRepository _buyerRepository;

    public OrderStartedDomainEventHandlers(IBuyerRepository buyerRepository)
    {
        _buyerRepository = buyerRepository;
    }

    public async Task Handle(OrderStartedDomainEvent notification, CancellationToken cancellationToken)
    {
        var cardTypeId = (notification.CardTypeId != 0) ? notification.CardTypeId : 1;
        
        var buyer = await _buyerRepository.GetSingleAsync(
            predicate:x=>x.Name == notification.UserName,
            includes:x => x.PaymentMethods,
            cancellationToken : cancellationToken);

        bool isExist = buyer != null;
        
        if (!isExist)
        {
            buyer = new Buyer(notification.UserName);
        }
        
        buyer.VerifyOrAddPaymentMethod(
            cardTypeId: cardTypeId,
            alias: $"PaymentMethod on {DateTime.UtcNow}",
            cardNumber: notification.CardNumber,
            securityNumber: notification.CardSecurityNumber,
            cardHolderName: notification.CardHolderName,
            expiration: notification.CardExpiration,
            orderId: notification.Order.Id);
        
        var buyerUpdated = isExist ?
            _buyerRepository.Update(buyer) :
            await _buyerRepository.AddAsync(buyer, cancellationToken);
        
        await _buyerRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
        
        // order status changed event maybe be fired here
    }
}