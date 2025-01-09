namespace ECommerceProductsAPI.Models;

public class Address
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public required string StreetAddress { get; set; }
    public string? BuildingNo { get; set; }
    public string? City { get; set; }
    public string? Region { get; set; }
    public required string Country { get; set; }
    public required string PostalCode { get; set; }

    public User User { get; set; } = null!;
}
