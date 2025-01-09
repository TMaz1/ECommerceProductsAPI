using System.Text.Json.Serialization;
using ECommerceProductsAPI.Models.Enums;

namespace ECommerceProductsAPI.Dtos.Subscription;

public class AddSubscriptionRequest
{
    public required int UserId { get; set; }
    public required int ProductId { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public required SubscriptionFrequencyType Frequency { get; set; }
}
