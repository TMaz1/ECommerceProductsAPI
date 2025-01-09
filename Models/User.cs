using System.ComponentModel.DataAnnotations;

namespace ECommerceProductsAPI.Models;

public class User
{
    public int Id { get; set; }

    [EmailAddress]
    public required string Email { get; set; }
    public required byte[] PasswordHash { get; set; }
    public required byte[] PasswordSalt { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? PhoneNumber { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public List<Address> Addresses { get; set; } = [];
    public List<UserSubscription> Subscriptions { get; set; } = [];
}