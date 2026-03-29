using ECommerceProductsAPI.Dtos.Address;
using ECommerceProductsAPI.Dtos.Subscription;
using ECommerceProductsAPI.Dtos.Users;
using ECommerceProductsAPI.Models;

namespace ECommerceProductsAPI.Utils.Mappers;

public static class UserMapper
{
    public static UserResponse MapToUserResponse(User user)
    {
        if (user == null)
        {
            throw new ArgumentNullException(nameof(user), "User cannot be null.");
        }
        
        return new UserResponse
        {
            Id = user.Id,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            PhoneNumber = user.PhoneNumber,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt,
            Addresses = user.Addresses.Select(MapToAddressResponse).ToList() ?? [],
            Subscriptions = user.Subscriptions.Where(us => us.Subscription != null).Select(us => MapToSubscriptionResponse(us.Subscription!)).ToList() ?? []
        };
    }

    private static AddressResponse MapToAddressResponse(Address address) => AddressMapper.MapToAddressResponse(address);
    private static SubscriptionResponse MapToSubscriptionResponse(Subscription subscription) => SubscriptionMapper.MapToSubscriptionResponse(subscription);
}
