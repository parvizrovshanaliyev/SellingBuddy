using System;
using System.Collections.Generic;
using System.Linq;

namespace WebApp.Domain.Basket;

public class Basket 
{
    public List<BasketItem> Items { get; set; } = new();

    public string BuyerId { get; set; }

    public decimal Total()
    {
        return Math.Round(Items.Sum(x => x.UnitPrice * x.Quantity), 2);
    }
}