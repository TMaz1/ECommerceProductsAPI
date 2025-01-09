namespace ECommerceProductsAPI.Dtos.Products.Requests;

public class FilterProductsRequest
{
    public string? AttributeType { get; set; }
    public string? AttributeValue { get; set; }
    public string? ProductType { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public string? ProductName { get; set; }
    public bool? IsSubscription { get; set; }
}