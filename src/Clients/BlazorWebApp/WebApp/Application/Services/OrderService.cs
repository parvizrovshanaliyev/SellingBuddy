using WebApp.Application.Services.Interfaces;
using WebApp.Domain.Basket;
using WebApp.Domain.Order;

namespace WebApp.Application.Services;

public class OrderService : IOrderService
{
    public BasketDto MapOrderToBasket(Order order)
    {
        order.CardExpirationApiFormat();
        
        return new BasketDto()
        {
            City = order.City,
            Street = order.Street,
            State = order.State,
            Country = order.Country,
            ZipCode = order.ZipCode,
            CardNumber = order.CardNumber,
            CardHolderName = order.CardHolderName,
            CardExpirationShort = order.CardExpirationShort,
            CardExpiration = order.CardExpiration,
            CardSecurityNumber = order.CardSecurityNumber,
            CardTypeId = order.CardTypeId,
            Buyer = order.Buyer
        };
    }
}