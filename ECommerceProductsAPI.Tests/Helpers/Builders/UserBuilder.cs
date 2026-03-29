using ECommerceProductsAPI.Models;

namespace ECommerceProductsAPI.Tests.Helpers.Builders;

public class UserBuilder
{
    private readonly User _user = new()
    {
        Email = "test@example.com",
        PasswordHash = [1],
        PasswordSalt = [1],
        FirstName = "John",
        LastName = "Doe",
        PhoneNumber = "1234567890",
        Addresses = [],
        Subscriptions = []
    };

    public UserBuilder WithEmail(string email)
    {
        _user.Email = email;
        return this;
    }

    public UserBuilder WithFirstName(string firstName)
    {
        _user.FirstName = firstName;
        return this;
    }

    public UserBuilder WithLastName(string lastName)
    {
        _user.LastName = lastName;
        return this;
    }

    public UserBuilder WithPassword(byte[] hash, byte[] salt)
    {
        _user.PasswordHash = hash;
        _user.PasswordSalt = salt;
        return this;
    }

    public UserBuilder AddAddress(Address address)
    {
        _user.Addresses.Add(address);
        return this;
    }

    public UserBuilder AddSubscription(UserSubscription subscription)
    {
        _user.Subscriptions.Add(subscription);
        return this;
    }

    public User Build() => _user;
}