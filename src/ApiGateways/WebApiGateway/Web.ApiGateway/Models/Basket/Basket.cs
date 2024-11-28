namespace Web.ApiGateway.Models.Basket;

public class Basket 
{
    public List<BasketItem> Items { get; set; } = new();

    public string BuyerId { get; set; }

    public decimal Total()
    {
        return Math.Round(Items.Sum(x => x.UnitPrice * x.Quantity), 2);
    }
}

public class BasketItem
{
    public string Id { get; set; }
    public int ProductId { get; set; }

    public string ProductName { get; set; }
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }

    public string PictureUrl { get; set; }
}