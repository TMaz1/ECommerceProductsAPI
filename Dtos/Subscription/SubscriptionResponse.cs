using ECommerceProductsAPI.Dtos.Products.Responses;

namespace ECommerceProductsAPI.Dtos.Subscription;

public class SubscriptionResponse
{
    public int Id { get; set; }
    public string Frequency { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime PreviousBillingDate { get; set; }
    public DateTime NextBillingDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public BasicProductResponse? ProductDetails { get; set; } = new BasicProductResponse();

}
