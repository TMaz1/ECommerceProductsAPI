using ECommerceProductsAPI.Models.Enums;

namespace ECommerceProductsAPI.Models;

public class Subscription
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public required SubscriptionFrequencyType Frequency { get; set; }
    public DateTime StartDate { get; set; } = DateTime.UtcNow;
    public DateTime PreviousBillingDate { get; set; }
    public DateTime NextBillingDate { get; set; }
    public required StatusType Status { get; set; }

    public Product Product { get; set; } = null!;
}
