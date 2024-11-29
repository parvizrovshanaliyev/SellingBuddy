using System;
using System.ComponentModel.DataAnnotations;

namespace WebApp.Domain.Basket;

public class BasketDto
{
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
    public string CardNumber { get; set; }
    
    [Required]
    public string CardHolderName { get; set; }
    
    [Required]
    public string CardExpirationShort { get; set; }
    
    [Required]
    public DateTime CardExpiration { get; set; }
    
    [Required]
    public string CardSecurityNumber { get; set; }
    
    public int CardTypeId { get; set; }
    
    public string Buyer { get; set; }
}