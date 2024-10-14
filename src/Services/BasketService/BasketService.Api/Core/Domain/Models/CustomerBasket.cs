using System.Collections.Generic;

namespace BasketService.Api.Core.Domain.Models;

public class CustomerBasket
{
    public string BuyerId { get; set; }
    public List<BasketItem> Items { get; set; }

    public CustomerBasket()
    {
        Items = new List<BasketItem>();
    }

    public CustomerBasket(string buyerId) : this()
    {
        BuyerId = buyerId;
    }
}
