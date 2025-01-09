namespace ECommerceProductsAPI.Dtos.Products.Requests.Variations;

public class AddVariationRequest
{
    public required int ProductId { get; set; }
    public required ProductVariationRequest Variation { get; set; }
    public required List<AddProductAttributeRequest> Attributes { get; set; }
}
