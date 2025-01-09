using ECommerceProductsAPI.Dtos;
using ECommerceProductsAPI.Dtos.Address;
using ECommerceProductsAPI.Dtos.Users;

namespace ECommerceProductsAPI.Services.Users.Addresses;

public interface IAddressService
{
    Task<ServiceResponse<List<AddressResponse>>> GetAddressesByUserId(int userId);
    Task<ServiceResponse<UserResponse>> AddAddress(int userId, AddAddressRequest addressRequest);
    Task<ServiceResponse<UserResponse>> UpdateAddress(int userId, int addressId, UpdateAddressRequest addressRequest);
    Task<ServiceResponse<UserResponse>> DeleteAddress(int userId, int addressId);
}