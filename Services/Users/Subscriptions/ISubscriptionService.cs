using ECommerceProductsAPI.Dtos;
using ECommerceProductsAPI.Dtos.Subscription;
using ECommerceProductsAPI.Dtos.Users;
using ECommerceProductsAPI.Models;

namespace ECommerceProductsAPI.Services.Users.Subscriptions;

public interface ISubscriptionService
{
    Task<ServiceResponse<List<SubscriptionResponse>>> GetSubscriptionsByUserId(int userId);
    Task<ServiceResponse<UserResponse>> AddUserSubscription(AddSubscriptionRequest subscriptionRequest);
    Task<ServiceResponse<UserResponse>> UpdateUserSubscription(UpdateSubscriptionRequest updatedRequest);
    Task<ServiceResponse<UserResponse>> DeleteUserSubscription(int userId, int subscriptionId);
    Task<ServiceResponse<UserResponse>> DeleteAllSubscriptionsForUser(int userId);
}