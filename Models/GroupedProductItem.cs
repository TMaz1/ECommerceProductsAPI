namespace ECommerceProductsAPI.Models;

public class GroupedProductItem
{
    public int Id { get; set; }
    public int GroupedProductId { get; set; }
    public int ProductItemId { get; set; }
    public int Quantity { get; set; }

    public Product ProductItem { get; set; } = null!;
    public Product GroupedProduct { get; set; } = null!;
}
