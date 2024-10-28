namespace OrderService.Application.Features.Order.Models;

public class OrderDetailResponse
{
    public string OrderNumber { get; set; }
    
    public DateTime Date { get; set; }
    
    public string Status { get; set; }
    
    public string Description { get; set; }
    
    public string Street { get; set; }
    
    public string City { get; set; }
    
    public string ZipCode { get; set; }
    
    public string Country { get; set; }
    
    public List<OrderItemModel> OrderItems { get; set; }
    
    public decimal Total { get; set; }
}

public class OrderItemModel
{
    public string ProductName { get; set; }
    
    public int Quantity { get; set; }
    
    public decimal UnitPrice { get; set; }
    
    public decimal PictureUrl { get; set; }
}

public class OrderItemDto
{
    private int _productId;

    /// <summary>
    /// Gets the product ID associated with the order item.
    /// </summary>
    public int ProductId
    {
        get => _productId;
        set => _productId = value;
    }

    /// <summary>
    /// Gets the name of the product.
    /// </summary>
    public string ProductName { get; set; }

    /// <summary>
    /// Gets the unit price of the product.
    /// </summary>
    public decimal UnitPrice { get; set; }

    /// <summary>
    /// Gets the quantity (units) of the product in the order.
    /// </summary>
    public int Units { get; set; }

    /// <summary>
    /// Gets the URL of the product image.
    /// </summary>
    public string PictureUrl { get; set; }
}