using ECommerceProductsAPI.Models;
using ECommerceProductsAPI.Models.Enums;

namespace ECommerceProductsAPI.Tests.Helpers.Builders;

public class SubscriptionBuilder
{
    private Subscription _subscription = new()
    {
        Frequency = SubscriptionFrequencyType.Monthly,
        Status = StatusType.Active,
        StartDate = DateTime.UtcNow,
        PreviousBillingDate = DateTime.UtcNow.AddMonths(-1),
        NextBillingDate = DateTime.UtcNow.AddMonths(1)
    };

    public SubscriptionBuilder ForProduct(Product product)
    {
        _subscription.Product = product;
        _subscription.ProductId = product.Id;
        return this;
    }

    public SubscriptionBuilder WithFrequency(SubscriptionFrequencyType frequency)
    {
        _subscription.Frequency = frequency;
        return this;
    }

    public SubscriptionBuilder WithStatus(StatusType status)
    {
        _subscription.Status = status;
        return this;
    }

    public Subscription Build() => _subscription;
}