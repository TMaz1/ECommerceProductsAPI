using ECommerceProductsAPI.Data;
using ECommerceProductsAPI.Dtos;
using ECommerceProductsAPI.Dtos.Address;
using ECommerceProductsAPI.Dtos.Users;
using ECommerceProductsAPI.Models;
using ECommerceProductsAPI.Repositories;
using ECommerceProductsAPI.Utils;
using Microsoft.EntityFrameworkCore;

namespace ECommerceProductsAPI.Services.Users.Addresses;
public class AddressService(ProductsDataContext context, UserRepository userRepository) : IAddressService
{
    private readonly ProductsDataContext _context = context;
    private readonly UserRepository _userRepository = userRepository;

    public async Task<ServiceResponse<List<AddressResponse>>> GetAddressesByUserId(int userId)
    {
        var response = new ServiceResponse<List<AddressResponse>>();

        try
        {
            var addresses = await _context.Addresses
                .AsNoTracking()
                .Where(a => a.UserId == userId)
                .Select(a => MapToAddressResponse(a))
                .AsQueryable()
                .ToListAsync();

            response.Data = addresses;
            response.Message = $"Successfully retrieved {addresses.Count} address(es) for user ID '{userId}'";
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = $"Error retrieving addresses: {ex.Message}";
        }

        return response;
    }

    public async Task<ServiceResponse<UserResponse>> AddAddress(int userId, AddAddressRequest addressRequest)
    {
        var response = new ServiceResponse<UserResponse>();

        try
        {
            bool userExists = await _context.Users.AnyAsync(u => u.Id == userId);
            
            if (!userExists)
            {
                throw new Exception($"User with ID {userId} does not exist.");
            }

            var address = new Address
            {
                UserId = userId,
                StreetAddress = addressRequest.StreetAddress,
                BuildingNo = addressRequest.BuildingNo,
                City = addressRequest.City,
                Region = addressRequest.Region,
                Country = addressRequest.Country,
                PostalCode = addressRequest.PostalCode
            };

            // _context.Addresses.Add(address);
            await _context.Addresses.AddAsync(address);
            await _context.SaveChangesAsync();

            var updatedUser = await _userRepository.GetUserDetailsByIdNoTracking(userId) ?? throw new Exception($"User with ID '{userId}' not found.");

            response.Data = MapToUserResponse(updatedUser);
            response.Message = $"Successfully added address for user ID '{userId}'";
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = $"Error adding address: {ex.Message}";
        }

        return response;
    }

    public async Task<ServiceResponse<UserResponse>> UpdateAddress(int userId, int addressId, UpdateAddressRequest addressRequest)
    {
        var response = new ServiceResponse<UserResponse>();

        try
        {
            var address = await GetAddressDetailsByIds(addressId, userId) ?? throw new Exception($"Address with ID '{addressId}' not found for user with ID '{userId}'.");

            if (!string.IsNullOrWhiteSpace(addressRequest.StreetAddress))
            {
                address.StreetAddress = addressRequest.StreetAddress;
            }

            if (!string.IsNullOrWhiteSpace(addressRequest.BuildingNo))
            {
                address.BuildingNo = addressRequest.BuildingNo;
            }

            if (!string.IsNullOrWhiteSpace(addressRequest.City))
            {
                address.City = addressRequest.City;
            }

            if (!string.IsNullOrWhiteSpace(addressRequest.Region))
            {
                address.Region = addressRequest.Region;
            }

            if (!string.IsNullOrWhiteSpace(addressRequest.Country))
            {
                address.Country = addressRequest.Country;
            }

            if (!string.IsNullOrWhiteSpace(addressRequest.PostalCode))
            {
                address.PostalCode = addressRequest.PostalCode;
            }

            await _context.SaveChangesAsync();

            var updatedUser = await _userRepository.GetUserDetailsByIdNoTracking(userId) ?? throw new Exception($"User with ID '{userId}' not found.");

            response.Data = MapToUserResponse(updatedUser);
            response.Message = $"Successfully updated address for user ID '{userId}'";
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = $"Error updating address: {ex.Message}";
        }

        return response;
    }

    public async Task<ServiceResponse<UserResponse>> DeleteAddress(int userId, int addressId)
    {
        var response = new ServiceResponse<UserResponse>();

        try
        {
            var address = await GetAddressDetailsByIds(addressId, userId) ?? throw new Exception($"Address ID {addressId} not found for user ID {userId}.");

            _context.Addresses.Remove(address);
            await _context.SaveChangesAsync();

            var updatedUser = await _userRepository.GetUserDetailsByIdNoTracking(userId) ?? throw new Exception($"User with ID '{userId}' not found.");

            response.Data = MapToUserResponse(updatedUser);
            response.Message = $"Successfully deleted address with ID '{addressId}' for user ID '{userId}'";
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = $"Error deleting address: {ex.Message}";
        }

        return response;
    }

    private async Task<Address?> GetAddressDetailsByIds(int addressId, int userId)
    {
        return await _context.Addresses.FirstOrDefaultAsync(a => a.Id == addressId && a.UserId == userId);
    }

    private static UserResponse MapToUserResponse(User user) => UserMapper.MapToUserResponse(user);
    private static AddressResponse MapToAddressResponse(Address address) => AddressMapper.MapToAddressResponse(address);
}