namespace ECommerceProductsAPI.Dtos.Address;

public class AddressResponse
{
    public int Id { get; set; }
    public string? StreetAddress { get; set; }
    public string? BuildingNo { get; set; }
    public string? City { get; set; }
    public string? Region { get; set; }
    public string? Country { get; set; }
    public string? PostalCode { get; set; }
}