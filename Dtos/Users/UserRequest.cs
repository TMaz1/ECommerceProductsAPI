using System.ComponentModel.DataAnnotations;
namespace ECommerceProductsAPI.Dtos.Users;

public class UserRequest
{
    [EmailAddress]
    public string? Email { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? PhoneNumber { get; set; }
}