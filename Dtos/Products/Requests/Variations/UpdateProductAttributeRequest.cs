namespace ECommerceProductsAPI.Dtos.Products.Requests.Variations;

public class UpdateProductAttributeRequest
{
    public int AttributeId { get; set; }
    public string? Type { get; set; }
    public string? Value { get; set; }
}