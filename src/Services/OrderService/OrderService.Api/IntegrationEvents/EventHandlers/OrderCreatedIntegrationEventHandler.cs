using System.Threading.Tasks;
using EventBus.Base.Abstraction;
using MediatR;
using Microsoft.Extensions.Logging;
using OrderService.Api.IntegrationEvents.Events;
using OrderService.Application.Features.Order.Commands;

namespace OrderService.Api.IntegrationEvents.EventHandlers;

public class OrderCreatedIntegrationEventHandler : IIntegrationEventHandler<OrderCreatedIntegrationEvent>
{
    private readonly IMediator _mediator;
    private readonly ILogger<OrderCreatedIntegrationEvent> _logger;

    public OrderCreatedIntegrationEventHandler(IMediator mediator,ILogger<OrderCreatedIntegrationEvent> logger )
    {
        _logger = logger;
        _mediator = mediator;
    }

    public async Task Handle(OrderCreatedIntegrationEvent @event)
    {
        _logger.LogInformation("----- Handling integration event: {IntegrationEventId} at {AppName} - ({@IntegrationEvent})",
            @event.Id,
            typeof(Program).Namespace,
            @event);
        
        var createOrderCommand = new CreateOrderCommand(
            basketItems: @event.Basket.Items,
            userId: @event.UserId,
            userName: @event.UserName,
            city: @event.City,
            street: @event.Street,
            state: @event.State,
            country: @event.Country,
            zipCode: @event.ZipCode,
            cardNumber: @event.CardNumber,
            cardHolderName: @event.CardHolderName,
            cardExpiration: @event.CardExpiration,
            cardSecurityNumber: @event.CardSecurityNumber,
            cardTypeId: @event.CardTypeId
        );
        
        await _mediator.Send(createOrderCommand);
    }
}