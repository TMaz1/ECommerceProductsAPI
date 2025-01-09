namespace ECommerceProductsAPI.Dtos.Products.Responses;

public class GroupedProductItemResponse
{
    public int Id { get; set; }
    public int Quantity { get; set; }
    public BasicProductResponse ProductDetails { get; set; } = new BasicProductResponse();
}
