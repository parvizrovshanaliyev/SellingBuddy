using System.Collections.Generic;
using System.Threading.Tasks;
using BasketService.Api.Core.Domain.Models;

namespace BasketService.Api.Core.Application.Repository;

public interface IBasketRepository
{
   Task<CustomerBasket> GetBasketAsync(string buyerId);
   IEnumerable<string> GetUsers();
   Task<CustomerBasket> UpdateBasketAsync(CustomerBasket basket);
   Task DeleteBasketAsync(string id);
}
