using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Shared.Caching;
using BasketService.Api.Core.Domain.Models;
using StackExchange.Redis;

namespace BasketService.Api.Core.Infrastructure.Repository;

public class BasketRepository : IBasketRepository
{
   private readonly IRedisService _redisService;

   public BasketRepository(IRedisService redisService)
   {
      _redisService = redisService;
   }

   public async Task<CustomerBasket> GetBasketAsync(string buyerId)
   {
      var basket = await _redisService.GetAsync<CustomerBasket>(buyerId);
      return basket;
   }

   public IEnumerable<string> GetUsers()
   {
      var server = _redisService.GetServer();
      
      var data = server.Keys();
      
      return data?.Select(k => k.ToString());
   }

   public async Task<CustomerBasket> UpdateBasketAsync(CustomerBasket basket)
   {
      await _redisService.SetAsync(basket.BuyerId, basket);

      return await GetBasketAsync(basket.BuyerId);
   }

   public async Task DeleteBasketAsync(string id)
   {
      await _redisService.DeleteAsync(id);
   }
}