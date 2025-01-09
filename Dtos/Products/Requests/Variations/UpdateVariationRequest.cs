namespace ECommerceProductsAPI.Dtos.Products.Requests.Variations;

public class UpdateVariationRequest
{
    public int ProductId { get; set; }
    public int VariationId { get; set; }
    public ProductVariationRequest? Variation { get; set; }
    public List<AddProductAttributeRequest>? NewAttributes { get; set; }
    public List<UpdateProductAttributeRequest>? CurrentAttributes { get; set; }
}
