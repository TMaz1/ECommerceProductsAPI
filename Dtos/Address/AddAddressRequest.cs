using System.ComponentModel.DataAnnotations;

namespace ECommerceProductsAPI.Dtos.Address;

public class AddAddressRequest
{
    [Required]
    [StringLength(255, ErrorMessage = "Street address cannot exceed 255 characters.")]
    public required string StreetAddress { get; set; }
    
    [StringLength(50, ErrorMessage = "Building number cannot exceed 50 characters.")]
    public string? BuildingNo { get; set; }
    
    [StringLength(100, ErrorMessage = "City name cannot exceed 100 characters.")]
    public string? City { get; set; }

    [StringLength(100, ErrorMessage = "Region name cannot exceed 100 characters.")]
    public string? Region { get; set; }

    [Required]
    [StringLength(100, ErrorMessage = "Country name cannot exceed 100 characters.")]
    public required string Country { get; set; }
    
    [Required]
    public required string PostalCode { get; set; }
}