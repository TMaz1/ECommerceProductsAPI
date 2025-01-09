using System.ComponentModel.DataAnnotations;

namespace ECommerceProductsAPI.Dtos.Address;

public class UpdateAddressRequest
{
    [StringLength(255, ErrorMessage = "Street address cannot exceed 255 characters.")]
    public string? StreetAddress { get; set; }

    [StringLength(50, ErrorMessage = "Building number cannot exceed 50 characters.")]
    public string? BuildingNo { get; set; }

    [StringLength(100, ErrorMessage = "City name cannot exceed 100 characters.")]
    public string? City { get; set; }

    [StringLength(100, ErrorMessage = "Region name cannot exceed 100 characters.")]
    public string? Region { get; set; }

    [StringLength(100, ErrorMessage = "Country name cannot exceed 100 characters.")]
    public string? Country { get; set; }

    
    public string? PostalCode { get; set; }
}