namespace WebApp.Domain.Basket;

public class BasketItem
{
    public int Id { get; set; }
    public int ProductId { get; set; }

    public string ProductName { get; set; }
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }

    public string PictureUrl { get; set; }
}