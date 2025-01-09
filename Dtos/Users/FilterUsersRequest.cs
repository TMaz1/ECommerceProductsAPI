namespace ECommerceProductsAPI.Dtos.Users;

public class FilterUsersRequest
{
    public string? Email { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? PhoneNumber { get; set; }
    public DateTime? CreatedAfter { get; set; }
    public DateTime? CreatedBefore { get; set; }
    public bool? HasSubscriptions { get; set; }
}
