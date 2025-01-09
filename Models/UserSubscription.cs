namespace ECommerceProductsAPI.Models;

public class UserSubscription
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int SubscriptionId { get; set; }

    public User User { get; set; } = null!;
    public Subscription Subscription { get; set; } = null!;
}
