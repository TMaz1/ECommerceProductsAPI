using ECommerceProductsAPI.Dtos.Address;
using ECommerceProductsAPI.Models;

namespace ECommerceProductsAPI.Utils.Mappers;

public static class AddressMapper
{
    public static AddressResponse MapToAddressResponse(Address address)
    {
        return new AddressResponse
        {
            Id = address.Id,
            StreetAddress = address.StreetAddress,
            BuildingNo = address.BuildingNo,
            City = address.City,
            Region = address.Region,
            Country = address.Country,
            PostalCode = address.PostalCode
        };
    }
}