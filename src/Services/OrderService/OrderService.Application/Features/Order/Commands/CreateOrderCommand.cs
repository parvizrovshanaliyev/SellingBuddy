
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventBus.Base.Abstraction;
using OrderService.Application.IntegrationEvents;
using OrderService.Application.Interfaces.Repositories;
using OrderService.Domain.AggregateModels.OrderAggregate;
using OrderService.Domain.Models;

namespace OrderService.Application.Features.Order.Commands;

public class CreateOrderCommand : IRequest<bool>
{
    public string UserName { get; set; }
    
    public string City { get; set; }
    
    public string Street { get; set; }
    
    public string State { get; set; }
    
    public string Country { get; set; }
    
    public string ZipCode { get; set; }
    
    public string CardNumber { get; set; }
    
    public string CardHolderName { get; set; }
    
    public DateTime CardExpiration { get; set; }
    
    public string CardSecurityNumber { get; set; }
    
    public int CardTypeId { get; set; }

    private List<OrderItemDto> _orderItems;
    
    public IEnumerable<OrderItemDto> OrderItems => _orderItems;
    
    public CreateOrderCommand()
    {
        _orderItems = new List<OrderItemDto>();
    }

    public CreateOrderCommand(
        List<BasketItem> basketItems,
        string userId,
        string userName,
        string city,
        string street,
        string state,
        string country,
        string zipCode,
        string cardNumber,
        string cardHolderName,
        DateTime cardExpiration,
        string cardSecurityNumber,
        int cardTypeId) : this()
    {
        var orderItems = basketItems.Select(item => new OrderItemDto
        {
            ProductId = item.ProductId,
            ProductName = item.ProductName,
            UnitPrice = item.UnitPrice,
            Units = item.Quantity,
            PictureUrl = item.PictureUrl
        });

        _orderItems = orderItems.ToList();

        UserName = userName;
        City = city;
        Street = street;
        State = state;
        Country = country;
        ZipCode = zipCode;
        CardNumber = cardNumber;
        CardHolderName = cardHolderName;
        CardExpiration = cardExpiration;
        CardSecurityNumber = cardSecurityNumber;
        CardTypeId = cardTypeId;
    }
}

public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, bool>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IEventBus _eventBus;

    public CreateOrderCommandHandler(IOrderRepository orderRepository,  IEventBus eventBus)
    {
        _orderRepository = orderRepository;
        _eventBus = eventBus;
    }

    public async Task<bool> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        var address = new Address(
            request.Street,
            request.City,
            request.State,
            request.Country,
            request.ZipCode);
        
        var order = new Domain.AggregateModels.OrderAggregate.Order(
            username: request.UserName,
            address: address,
            cardTypeId: request.CardTypeId,
            cardNumber: request.CardNumber,
            cardSecurityNumber: request.CardSecurityNumber,
            cardHolderName: request.CardHolderName, 
            cardExpiration: request.CardExpiration,
            null);

        foreach (var item in request.OrderItems)
        {
            order.AddOrderItem(item.ProductId, item.ProductName, item.UnitPrice, item.PictureUrl, item.Units);
        }
        
        await _orderRepository.AddAsync(order, cancellationToken);
        await _orderRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
        
        var orderStartedIntegrationEvent = new OrderStartedIntegrationEvent(userName: request.UserName);
        
        _eventBus.Publish(orderStartedIntegrationEvent);
        
        return true;
    }
}