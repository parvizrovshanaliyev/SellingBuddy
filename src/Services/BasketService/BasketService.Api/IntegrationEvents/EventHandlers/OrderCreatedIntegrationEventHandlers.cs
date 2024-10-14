using System.Threading.Tasks;
using BasketService.Api.IntegrationEvents.Events;
using EventBus.Base.Abstraction;

namespace BasketService.Api.IntegrationEvents.EventHandlers;

public class OrderCreatedIntegrationEventHandlers : IIntegrationEventHandler<OrderCreatedIntegrationEvent>
{
    private readonly IBasketRepository _basketRepository;
    private readonly ILogger<OrderCreatedIntegrationEvent> _logger;

    public OrderCreatedIntegrationEventHandlers(IBasketRepository basketRepository, ILogger<OrderCreatedIntegrationEvent> logger)
    {
        _basketRepository = basketRepository;
        _logger = logger;
    }

    public async Task Handle(OrderCreatedIntegrationEvent @event)
    {
        _logger.LogInformation(
            "-------- Handling integration event: {IntegrationEvent} at BasketService.Api - ({@IntegrationEvent})",
            @event.Id,
            @event);

        await _basketRepository.DeleteBasketAsync(@event.UserId.ToString());
    }
}