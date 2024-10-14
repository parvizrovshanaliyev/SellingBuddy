using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BasketService.Api.Core.Domain.Models;

public class BasketItem : IValidatableObject
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal OldUnitPrice { get; set; }
    public int Quantity { get; set; }
    public string PictureUrl { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (Quantity <= 0)
        {
            yield return new ValidationResult("Quantity must be greater than zero.", new[] { nameof(Quantity) });
        }

        if (UnitPrice < 0)
        {
            yield return new ValidationResult("Unit price cannot be negative.", new[] { nameof(UnitPrice) });
        }

        if (OldUnitPrice < 0)
        {
            yield return new ValidationResult("Old unit price cannot be negative.", new[] { nameof(OldUnitPrice) });
        }
    }
}