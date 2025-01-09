using System.Text.Json;
using ECommerceProductsAPI.Dtos;
using ECommerceProductsAPI.Dtos.Subscription;
using ECommerceProductsAPI.Dtos.Users;
using ECommerceProductsAPI.Services.Caching;
using ECommerceProductsAPI.Services.Users;
using ECommerceProductsAPI.Services.Users.Register;
using ECommerceProductsAPI.Services.Users.Subscriptions;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceProductsAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController(IUserService userService, IUserRegisterService userRegisterService, ISubscriptionService subscriptionService, IRedisCacheService cache) : ControllerBase
{
    private readonly IUserService _userService = userService;
    private readonly IUserRegisterService _userRegisterService = userRegisterService;
    private readonly ISubscriptionService _subscriptionService = subscriptionService;
    private readonly IRedisCacheService _cache = cache;
    const string allUsersCacheKey = "users";
    const string userKeyPrefix = "user_";

    [HttpGet]
    public async Task<ActionResult<ServiceResponse<List<UserResponse>>>> GetAllUsers()
    {
        var response = _cache.GetData<ServiceResponse<List<UserResponse>>>(allUsersCacheKey);

        if (response is not null)
        {
            return Ok(response);
        }

        response = await _userService.GetAllUsers();

        if (response.Success)
        {
            _cache.SetData(allUsersCacheKey, response);
        }

        return Ok(response);
    }

    [HttpGet("page={pageNumber}")]
    public async Task<ActionResult<ServiceResponse<PaginatedResponse<UserResponse>>>> GetAllUsersWithPagination(int pageNumber, int totalItemsPerPage)
    {
        string cachingKey = $"products_page_{pageNumber}";

        var response = _cache.GetData<ServiceResponse<PaginatedResponse<UserResponse>>>(cachingKey);

        if (response is not null)
        {
            return Ok(response);
        }

        response = await _userService.GetUsersWithPagination(pageNumber, totalItemsPerPage);

        if (response.Success)
        {
            _cache.SetData(cachingKey, response);
        }

        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ServiceResponse<UserResponse>>> GetUserById(int id)
    {
        string cachingKey = $"{userKeyPrefix}{id}";
        var response = _cache.GetData<ServiceResponse<UserResponse>>(cachingKey);

        if (response is not null)
        {
            return Ok(response);
        }

        response = await _userService.GetUserById(id);

        if (response.Success && response.Data != null)
        {
            _cache.SetData(cachingKey, response);
        }

        return Ok(response);
    }

    [HttpPost("search")]
    public async Task<ActionResult<ServiceResponse<List<UserResponse>>>> GetUserByFilter(FilterUsersRequest searchRequest)
    {
        var cachingKey = JsonSerializer.Serialize(searchRequest); // key generated from the search input so if the same search input is searched, the cached result is returned
        var response = _cache.GetData<ServiceResponse<List<UserResponse>>>(cachingKey);

        if (response is not null)
        {
            return Ok(response);
        }

        response = await _userService.GetUserByFilter(searchRequest);

        if (response.Success)
        {
            _cache.SetData(cachingKey, response);
        }

        return Ok(response);
    }

    [HttpPost("search/page={pageNumber}")]
    public async Task<ActionResult<ServiceResponse<PaginatedResponse<UserResponse>>>> GetProductsByFilterWithPagination(int pageNumber, int totalItemsPerPage, FilterUsersRequest queryFilter)
    {
        string serialisedRequest = JsonSerializer.Serialize(queryFilter);
        string cachingKey = $"users_search_{pageNumber}_{serialisedRequest}";

        var response = _cache.GetData<ServiceResponse<PaginatedResponse<UserResponse>>>(cachingKey);

        if (response is not null)
        {
            return Ok(response);
        }

        response = await _userService.GetUsersByFilterWithPagination(pageNumber, totalItemsPerPage, queryFilter);

        if (response.Success)
        {
            _cache.SetData(cachingKey, response);
        }

        return Ok(response);
    }


    [HttpPost("register")]
    public async Task<ActionResult<ServiceResponse<UserResponse>>> Register([FromBody] UserRegister userRequest)
    {
        var response = await _userRegisterService.Register(userRequest);

        if (response.Success && response.Data != null)
        {
            _cache.SetData($"{userKeyPrefix}{response.Data.Id}", response);
            _cache.RemoveData(allUsersCacheKey);
        }

        return response.Success ? Ok(response) : NotFound(response);
    }

    [HttpPost("login")]
    public async Task<ActionResult<ServiceResponse<UserResponse>>> Login(UserLogin userLogin)
    {
        var response = await _userRegisterService.Login(userLogin);

        if (response.Success && response.Data != null)
        {
            _cache.SetData($"{userKeyPrefix}{response.Data.Id}", response);
        }

        return response.Success ? Ok(response) : NotFound(response);
    }

    [HttpPut]
    public async Task<ActionResult<ServiceResponse<UserResponse>>> UpdateUser(int id, [FromBody] UserRequest userRequest)
    {
        var response = await _userService.UpdateUser(id, userRequest);
        
        if (response.Success && response.Data != null)
        {
            InvalidateUserCache(id);
            _cache.SetData($"{userKeyPrefix}{id}", response);
        }
        
        return response.Success ? Ok(response) : NotFound(response);
    }

    [HttpPost("{id}/reset-password")]
    public async Task<ActionResult<ServiceResponse<bool>>> UpdatePassword(int id, [FromBody] UpdatePasswordRequest passwordRequest)
    {
        var response = await _userService.UpdatePassword(id, passwordRequest);
        return response.Success ? Ok(response) : NotFound(response);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ServiceResponse<string>>> DeleteUser(int id)
    {
        var response = await _userService.DeleteUser(id);

        if (response.Success && response.Data != null)
        {
            InvalidateUserCache(id);
        }

        return response.Success ? Ok(response) : NotFound(response);
    }



    [HttpGet("subscription/{userId}")]
    public async Task<ActionResult<ServiceResponse<List<SubscriptionResponse>>>> GetSubscriptionsByUserId(int userId)
    {
        string cachingKey = $"{userKeyPrefix}{userId}_subscriptions";
        var response = _cache.GetData<ServiceResponse<List<SubscriptionResponse>>>(cachingKey);

        if (response is not null)
        {
            return Ok(response);
        }

        response = await _subscriptionService.GetSubscriptionsByUserId(userId);

        if (response.Success)
        {
            _cache.SetData(cachingKey, response);
        }

        return response.Success ? Ok(response) : NotFound(response);
    }

    [HttpPost("subscription")]
    public async Task<ActionResult<ServiceResponse<UserResponse>>> AddUserSubscription(AddSubscriptionRequest subscriptionRequest)
    {
        var response = await _subscriptionService.AddUserSubscription(subscriptionRequest);

        if (response.Success && response.Data != null)
        {
            InvalidateUserCache(subscriptionRequest.UserId);
            _cache.SetData($"{userKeyPrefix}{subscriptionRequest.UserId}", response);
        }

        return response.Success ? Ok(response) : NotFound(response);
    }

    [HttpPut("subscription")]
    public async Task<ActionResult<ServiceResponse<UserResponse>>> UpdateUserSubscription(UpdateSubscriptionRequest updatedRequest)
    {
        var response = await _subscriptionService.UpdateUserSubscription(updatedRequest);
        
        if (response.Success)
        {
            InvalidateUserCache(updatedRequest.UserId);
            _cache.SetData($"{userKeyPrefix}{updatedRequest.UserId}", response);
        }
        
        return response.Success ? Ok(response) : NotFound(response);
    }

    [HttpDelete("subscription/{userId}/{subscriptionId}")]
    public async Task<ActionResult<ServiceResponse<UserResponse>>> DeleteUserSubscription(int userId, int subscriptionId)
    {
        var response = await _subscriptionService.DeleteUserSubscription(userId, subscriptionId);

        if (response.Success)
        {
            InvalidateUserCache(userId);
        }

        return response.Success ? Ok(response) : NotFound(response);
    }

    [HttpDelete("subscription/all/{userId}")]
    public async Task<ActionResult<ServiceResponse<UserResponse>>> DeleteAllSubscriptionsForUser(int userId)
    {
        var response = await _subscriptionService.DeleteAllSubscriptionsForUser(userId);

        if (response.Success)
        {
            InvalidateUserCache(userId);
            _cache.SetData($"{userKeyPrefix}{userId}", response);
        }

        return response.Success ? Ok(response) : NotFound(response);
    }

    private void InvalidateUserCache(int userId)
    {
        _cache.RemoveData($"{userKeyPrefix}{userId}");
        _cache.RemoveData(allUsersCacheKey);
    }
}