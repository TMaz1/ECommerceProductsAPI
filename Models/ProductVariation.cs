using System.ComponentModel.DataAnnotations.Schema;

namespace ECommerceProductsAPI.Models;

public class ProductVariation
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public string? SKU { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal Price { get; set; }
    public int Stock { get; set; }

    public Product Product { get; set; } = null!;
    public List<ProductVariationAttribute> VariationAttributes { get; set; } = [];
}
