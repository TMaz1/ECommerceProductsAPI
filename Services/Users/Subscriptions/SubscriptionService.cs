using ECommerceProductsAPI.Data;
using ECommerceProductsAPI.Dtos;
using ECommerceProductsAPI.Dtos.Subscription;
using ECommerceProductsAPI.Dtos.Users;
using ECommerceProductsAPI.Models;
using ECommerceProductsAPI.Models.Enums;
using ECommerceProductsAPI.Repositories;
using ECommerceProductsAPI.Utils;
using Microsoft.EntityFrameworkCore;

namespace ECommerceProductsAPI.Services.Users.Subscriptions;
public class SubscriptionService(ProductsDataContext context, UserRepository userRepository) : ISubscriptionService
{
    private readonly ProductsDataContext _context = context;
    private readonly UserRepository _userRepository = userRepository;

    public async Task<ServiceResponse<List<SubscriptionResponse>>> GetSubscriptionsByUserId(int userId)
    {
        var response = new ServiceResponse<List<SubscriptionResponse>>();

        try
        {
            var user = await _userRepository.GetUserDetailsByIdNoTracking(userId) ?? throw new Exception($"User with ID '{userId}' not found.");

            var subscriptions = await _context.Subscriptions
                .AsNoTracking()
                .Include(s => s.Product)
                .Where(s => _context.UserSubscriptions
                    .Any(us => us.UserId == userId && us.SubscriptionId == s.Id))
                .Select(s => MapToSubscriptionResponse(s))
                .ToListAsync();

            response.Data = subscriptions;
            response.Message = $"Successfully retrieved {subscriptions.Count} subscriptions for user with ID '{userId}'";
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = $"Error retrieving subscriptions: {ex.Message}";
        }
        return response;
    }

    public async Task<ServiceResponse<UserResponse>> AddUserSubscription(AddSubscriptionRequest subscriptionRequest)
    {
        var response = new ServiceResponse<UserResponse>();
        var activeStatus = StatusType.Active;

        try
        {
            var user = await _userRepository.GetUserDetailsByIdWithTracking(subscriptionRequest.UserId) ?? throw new Exception($"User with ID '{subscriptionRequest.UserId}' not found.");
            
            var product = await _userRepository.GetProductDetailsByIdWithTracking(subscriptionRequest.ProductId) ?? throw new Exception($"Product with ID '{subscriptionRequest.ProductId}' not found.");

            if (product.IsSubscription == false)
            {
                throw new Exception($"Product with ID {product.Id} does not offer subscriptions.");
            }


            if (user.Subscriptions != null && user.Subscriptions.Any(s => s.Subscription.Product.Id == subscriptionRequest.ProductId && s.Subscription.Frequency == subscriptionRequest.Frequency))
            {
                throw new Exception($"Subscription with Product ID '{subscriptionRequest.ProductId}' already exists.");
            }

            var newSubscription = new Subscription
            {
                ProductId = subscriptionRequest.ProductId,
                Frequency = subscriptionRequest.Frequency,
                NextBillingDate = CalculateNextBillingDate(DateTime.UtcNow, subscriptionRequest.Frequency, activeStatus),
                Status = activeStatus
            };

            await _context.Subscriptions.AddAsync(newSubscription);
            await _context.SaveChangesAsync();

            // add user-subscription
            var userSubscription = new UserSubscription
            {
                UserId = subscriptionRequest.UserId,
                SubscriptionId = newSubscription.Id
            };

            await _context.UserSubscriptions.AddAsync(userSubscription);
            await _context.SaveChangesAsync();

            var updatedUser = await _userRepository.GetUserDetailsByIdNoTracking(user.Id) ?? throw new Exception($"Failed to retrieve the newly added subscription with ID '{newSubscription.Id}'.");

            response.Data = MapToUserResponse(updatedUser);
            response.Message = $"Subscription with ID '{newSubscription.Id}' added successfully.";

        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = $"Error adding subscription: {ex.Message}";
        }

        return response;
    }

    public async Task<ServiceResponse<UserResponse>> UpdateUserSubscription(UpdateSubscriptionRequest updatedRequest)
    {
        var response = new ServiceResponse<UserResponse>();

        try
        {
            // validate user-subscription relationship and the subscription
            var userSubscription = await GetUserSubscriptionByIds(updatedRequest.SubscriptionId, updatedRequest.UserId) ?? throw new Exception($"Subscription with ID '{updatedRequest.SubscriptionId}' for user with ID '{updatedRequest.UserId}' not found.");

            var subscription = await GetSubscriptionDetailsById(updatedRequest.SubscriptionId) ?? throw new Exception($"Subscription with ID '{updatedRequest.SubscriptionId}' not found.");


            if (Enum.IsDefined(typeof(SubscriptionFrequencyType), updatedRequest.Frequency))
            {
                subscription.Frequency = updatedRequest.Frequency;
            }

            if (Enum.IsDefined(typeof(StatusType), updatedRequest.Status))
            {
                subscription.Status = updatedRequest.Status;

            }

            if (Enum.IsDefined(typeof(SubscriptionFrequencyType), updatedRequest.Frequency) && Enum.IsDefined(typeof(StatusType), updatedRequest.Status))
            {
                var previousBillingDate = (subscription.PreviousBillingDate == DateTime.MinValue) ? DateTime.UtcNow : subscription.PreviousBillingDate;
                subscription.NextBillingDate = CalculateNextBillingDate(previousBillingDate, updatedRequest.Frequency, updatedRequest.Status);
            }

            _context.Subscriptions.Update(subscription);
            await _context.SaveChangesAsync();

            var updatedUser = await _userRepository.GetUserDetailsByIdNoTracking(updatedRequest.UserId) ?? throw new Exception($"Updated user with ID '{updatedRequest.UserId}' not found.");

            response.Data = MapToUserResponse(updatedUser);
            response.Message = $"Successfully updated product with ID '{updatedRequest.UserId}'";
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = $"Error updating subscription: {ex.Message}";
        }

        return response;
    }

    public async Task<ServiceResponse<UserResponse>> DeleteUserSubscription(int userId, int subscriptionId)
    {
        var response = new ServiceResponse<UserResponse>();

        try
        {
            var userSubscription = await GetUserSubscriptionByIds(subscriptionId, userId) ?? throw new Exception($"Subscription ID '{subscriptionId}' for user ID '{userId}' not found.");

            var subscription = await GetSubscriptionDetailsById(subscriptionId) ?? throw new Exception($"Subscription with ID '{subscriptionId}' not found.");

            _context.UserSubscriptions.Remove(userSubscription);
            _context.Subscriptions.Remove(subscription);

            await _context.SaveChangesAsync();

            var updatedUser = await _userRepository.GetUserDetailsByIdNoTracking(userId) ?? throw new Exception($"Updated user with ID '{userId}' not found.");

            response.Data = MapToUserResponse(updatedUser);
            response.Message = $"Successfully deleted subscription ID '{subscriptionId}' for user ID '{userId}'.";
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = $"Error deleting subscription: {ex.Message}";
        }

        return response;
    }

    public async Task<ServiceResponse<UserResponse>> DeleteAllSubscriptionsForUser(int userId)
    {
        var response = new ServiceResponse<UserResponse>();

        try
        {
            var user = await _userRepository.GetUserDetailsByIdWithTracking(userId) ?? throw new Exception($"User with ID '{userId}' not found.");

            // if the user has any subscriptions
            if (user.Subscriptions == null || user.Subscriptions.Count == 0)
            {
                response.Message = $"No subscriptions found for user ID '{userId}'.";
                return response;
            }

            // get all subscriptions for the user & delete all UserSubscription instances for the user
            var subscriptionIds = user.Subscriptions.Select(us => us.SubscriptionId).ToList();
            _context.UserSubscriptions.RemoveRange(user.Subscriptions);

            // remove all associated subscriptions from the Subscriptions table
            var allSubscriptions = await _context.Subscriptions.Where(s => subscriptionIds.Contains(s.Id)).ToListAsync();

            _context.Subscriptions.RemoveRange(allSubscriptions);
            await _context.SaveChangesAsync();

            var updatedUser = await _userRepository.GetUserDetailsByIdNoTracking(userId) ?? throw new Exception($"Updated user with ID '{userId}' not found.");

            response.Data = MapToUserResponse(updatedUser);
            response.Message = $"Successfully deleted all subscriptions for user with ID '{userId}'";
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = $"Error deleting subscriptions for user with ID '{userId}': {ex.Message}";
        }

        return response;
    }


    private static DateTime CalculateNextBillingDate(DateTime previousBillingDate, SubscriptionFrequencyType frequency, StatusType status)
    {
        if (status == StatusType.Active)
        {
            if (frequency == SubscriptionFrequencyType.Annual)
            {
                return previousBillingDate.AddYears(1);
            }
            else if (frequency == SubscriptionFrequencyType.Monthly)
            {
                return previousBillingDate.AddMonths(1);
            }
            else
            {
                throw new Exception($"Unsupported frequency type: {nameof(frequency)}");
            }
        }
        else if (status == StatusType.Paused)
        {
            return previousBillingDate;
        }
        else if (status == StatusType.Cancelled)
        {
            // 0001-01-01T00:00:00 as next billing date
            return DateTime.MinValue;
        }
        else
        {
            throw new Exception($"Unsupported status type: {nameof(status)}");
        }
        
    }

    private async Task<Subscription?> GetSubscriptionDetailsById(int subscriptionId)
    {
        return await _context.Subscriptions.FirstOrDefaultAsync(s => s.Id == subscriptionId);
    }

    private async Task<UserSubscription?> GetUserSubscriptionByIds(int subscriptionId, int userId)
    {
        return await _context.UserSubscriptions.FirstOrDefaultAsync(us => us.SubscriptionId == subscriptionId && us.UserId == userId);
    }

    private static UserResponse MapToUserResponse(User user) => UserMapper.MapToUserResponse(user);
    private static SubscriptionResponse MapToSubscriptionResponse(Subscription subscription) => SubscriptionMapper.MapToSubscriptionResponse(subscription);
}