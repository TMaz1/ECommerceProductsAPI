using System.Text.Json.Serialization;

namespace ECommerceProductsAPI.Models.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum StatusType
{
    Active, 
    Paused,
    Cancelled
}