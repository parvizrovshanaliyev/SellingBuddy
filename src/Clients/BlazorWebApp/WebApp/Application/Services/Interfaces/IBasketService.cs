using System.Threading.Tasks;
using WebApp.Domain.Basket;

namespace WebApp.Application.Services.Interfaces;

public interface IBasketService
{
    Task<Basket> GetBasketAsync();
    Task<Basket> UpdateBasketAsync(Basket basket);
    Task AddItemToBasketAsync(int productId);
    Task<bool> CheckoutAsync(BasketDto basket);
}