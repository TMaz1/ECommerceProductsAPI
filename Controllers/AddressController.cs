using ECommerceProductsAPI.Dtos;
using ECommerceProductsAPI.Dtos.Address;
using ECommerceProductsAPI.Dtos.Users;
using ECommerceProductsAPI.Services.Caching;
using ECommerceProductsAPI.Services.Users.Addresses;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceProductsAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AddressController(IAddressService addressService, IRedisCacheService cache) : ControllerBase
{
    private readonly IAddressService _addressService = addressService;
    private readonly IRedisCacheService _cache = cache;
    const string allUsersCacheKey = "users";

    [HttpGet]
    public async Task<ActionResult<ServiceResponse<List<AddressResponse>>>> GetAddressesByUserId(int userId)
    {
        string cachingKey = $"addresses_{userId}";
        var response = _cache.GetData<ServiceResponse<List<AddressResponse>>>(cachingKey);

        if (response is not null)
        {
            return Ok(response);
        }

        response = await _addressService.GetAddressesByUserId(userId);

        if (response.Success && response.Data != null)
        {
            _cache.SetData(cachingKey, response);
        }

        return Ok(response);
    }

    [HttpPost]
    public async Task<ActionResult<ServiceResponse<UserResponse>>> AddAddress(int userId, [FromBody] AddAddressRequest addressRequest)
    {
        var response = await _addressService.AddAddress(userId, addressRequest);

        // clear cache for user list
        if (response.Success)
        {
            _cache.RemoveData($"user_{userId}");
            _cache.RemoveData(allUsersCacheKey);
        }

        return response.Success ? Ok(response) : NotFound(response);
    }

    [HttpPut]
    public async Task<ActionResult<ServiceResponse<UserResponse>>> UpdateAddress(int userId, int addressId, [FromBody] UpdateAddressRequest addressRequest)
    {
        var response = await _addressService.UpdateAddress(userId, addressId, addressRequest);
        
        // clear necessary caches
        if (response.Success)
        {
            _cache.RemoveData($"user_{userId}");
            _cache.RemoveData(allUsersCacheKey);
        }
        
        return response.Success ? Ok(response) : NotFound(response);
    }

    [HttpDelete]
    public async Task<ActionResult<ServiceResponse<UserResponse>>> DeleteAddress(int userId, int addressId)
    {
        var response = await _addressService.DeleteAddress(userId, addressId);

        // clear necessary caches
        if (response.Success)
        {
            _cache.RemoveData($"user_{userId}");
            _cache.RemoveData(allUsersCacheKey);
        }

        return response.Success ? Ok(response) : NotFound(response);
    }
}