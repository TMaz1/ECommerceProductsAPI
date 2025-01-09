using System.ComponentModel.DataAnnotations;

namespace ECommerceProductsAPI.Dtos.Users;

public class UserLogin
{
    [Required(ErrorMessage="Please fill the '{0}' field"), EmailAddress]
    public string Email { get; set; } = string.Empty;
    [Required(ErrorMessage="Please fill the '{0}' field")]
    public string Password { get; set; } = string.Empty;
}