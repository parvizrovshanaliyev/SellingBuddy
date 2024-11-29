using WebApp.Domain.Basket;
using WebApp.Domain.Order;

namespace WebApp.Application.Services.Interfaces;

public interface IOrderService
{
    BasketDto MapOrderToBasket(Order order);
}