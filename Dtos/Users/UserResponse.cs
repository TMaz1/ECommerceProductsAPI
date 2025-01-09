using System.ComponentModel.DataAnnotations;
using ECommerceProductsAPI.Dtos.Address;
using ECommerceProductsAPI.Dtos.Subscription;

namespace ECommerceProductsAPI.Dtos.Users;

public class UserResponse
{
    public int Id { get; set; }
    
    [EmailAddress]
    public required string Email { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? PhoneNumber { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public List<AddressResponse> Addresses { get; set; } = [];
    public List<SubscriptionResponse>? Subscriptions { get; set; }
}