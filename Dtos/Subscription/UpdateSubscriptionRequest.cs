using System.Text.Json.Serialization;
using ECommerceProductsAPI.Models.Enums;

namespace ECommerceProductsAPI.Dtos.Subscription;

public class UpdateSubscriptionRequest
{
    public int  UserId { get; set; }
    public int SubscriptionId { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public SubscriptionFrequencyType Frequency { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public StatusType Status { get; set; }
}
