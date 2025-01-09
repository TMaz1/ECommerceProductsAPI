using System.ComponentModel.DataAnnotations.Schema;

namespace ECommerceProductsAPI.Dtos.Products.Requests.Variations;

public class ProductVariationRequest
{
    public string? SKU { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal Price { get; set; }
    public int Stock { get; set; }
}