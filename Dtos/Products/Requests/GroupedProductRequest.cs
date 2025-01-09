namespace ECommerceProductsAPI.Dtos.Products.Requests;

public class GroupedProductRequest
{
    public int GroupedProductId { get; set; }
    public int ProductItemId { get; set; }
    public int Quantity { get; set; }
}