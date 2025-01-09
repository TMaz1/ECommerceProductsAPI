namespace ECommerceProductsAPI.Dtos.Products.Requests.Variations;

public class AddProductAttributeRequest
{
    public required string Type { get; set; }
    public required string Value { get; set; }
}