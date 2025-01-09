using ECommerceProductsAPI.Dtos.Products.Responses;
using ECommerceProductsAPI.Dtos.Subscription;
using ECommerceProductsAPI.Models;

namespace ECommerceProductsAPI.Utils;

public static class SubscriptionMapper
{
    public static SubscriptionResponse MapToSubscriptionResponse(Subscription subscription)
    {
        if (subscription == null)
        {
            throw new ArgumentNullException(nameof(subscription), "Subscription cannot be null.");
        }

        return new SubscriptionResponse
        {
            Id = subscription.Id,
            Frequency = subscription.Frequency.ToString(),
            StartDate = subscription.StartDate,
            PreviousBillingDate = subscription.PreviousBillingDate,
            NextBillingDate = subscription.NextBillingDate,
            Status = subscription.Status.ToString(),
            ProductDetails = (subscription.Product != null) ? MapToBasicProductResponse(subscription.Product) : null
        };
    }

    private static BasicProductResponse MapToBasicProductResponse(Product product) => ProductMapper.MapToBasicProductResponse(product);
    // private static BasicProductResponse? MapToBasicProductResponse(Product product) => (product != null) ? ProductMapper.MapToBasicProductResponse(product) : null;
}