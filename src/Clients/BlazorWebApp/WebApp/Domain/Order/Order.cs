using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace WebApp.Domain.Order;

public class Order
{
    public string OrderNumber { get; set; }
    
    public DateTime Date { get; set; }
    
    public string Status { get; set; }
    
    public decimal Total { get; set; }
    
    public string Description { get; set; }
    
    [Required]
    public string City { get; set; }
    
    [Required]
    public string Street { get; set; }
    
    [Required]
    public string State { get; set; }
    
    [Required]
    public string Country { get; set; }
    
    [Required]
    public string ZipCode { get; set; }
    
    [Required]
    [DisplayName("Card Number")]
    public string CardNumber { get; set; }
    
    [Required]
    [DisplayName("Card Holder Name")]
    public string CardHolderName { get; set; }
    
    public DateTime CardExpiration { get; set; }
    
    [RegularExpression(@"(0[1-9]|1[0-2])\/[0-9]{2}", ErrorMessage = "Expiration should match MM/YY format")]
    [DisplayName("Card Expiration")]
    public string CardExpirationShort { get; set; }
    
    [Required]
    [DisplayName("Card Security Number")]
    public string CardSecurityNumber { get; set; }
    
    public int CardTypeId { get; set; }
    
    public string Buyer { get; set; }
    
    public List<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    
    public void CardExpirationApiFormat()
    {
        var month = CardExpirationShort.Split('/')[0];
        var year = CardExpirationShort.Split('/')[1];
        
        CardExpiration = new DateTime(2000 + int.Parse(year), int.Parse(month), 1);
    }
}