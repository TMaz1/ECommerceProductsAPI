using ECommerceProductsAPI.Models;
using ECommerceProductsAPI.Models.Enums;

namespace ECommerceProductsAPI.Tests.Helpers;

public static class TestDataFactory
{
    // ----------------------------
    // USERS
    // ----------------------------
    public static User CreateUser(
        int id = 1,
        Action<User>? overrideAction = null)
    {
        var user = new User
        {
            Id = id,
            Email = $"user{id}@test.com",
            PasswordHash = [0x01],
            PasswordSalt = [0x01],
            FirstName = "Test",
            LastName = "User",
            PhoneNumber = "0000000000",
            Addresses = [],
            Subscriptions = []
        };

        overrideAction?.Invoke(user);
        return user;
    }

    public static Address CreateAddress(
        int userId,
        int id = 1,
        Action<Address>? overrideAction = null)
    {
        var address = new Address
        {
            Id = id,
            UserId = userId,
            StreetAddress = "123 Test St",
            City = "London",
            Country = "UK",
            PostalCode = "SW1A 1AA"
        };

        overrideAction?.Invoke(address);
        return address;
    }

    // ----------------------------
    // PRODUCTS
    // ----------------------------
    public static Product CreateProduct(
        int id = 1,
        ProductType type = ProductType.Simple,
        Action<Product>? overrideAction = null)
    {
        var product = new Product
        {
            Id = id,
            Name = $"Product{id}",
            Price = 10,
            ProductType = type,
            Variations = [],
            GroupedProductItems = []
        };

        overrideAction?.Invoke(product);
        return product;
    }

    public static ProductVariation CreateVariation(
        int id = 1,
        decimal price = 10,
        int stock = 10,
        Action<ProductVariation>? overrideAction = null)
    {
        var variation = new ProductVariation
        {
            Id = id,
            SKU = $"SKU{id}",
            Price = price,
            Stock = stock,
            VariationAttributes = []
        };

        overrideAction?.Invoke(variation);
        return variation;
    }

    public static ProductAttribute CreateAttribute(
        int id = 1,
        string type = "Color",
        string value = "Red",
        Action<ProductAttribute>? overrideAction = null)
    {
        var attr = new ProductAttribute
        {
            Id = id,
            Type = type,
            Value = value
        };

        overrideAction?.Invoke(attr);
        return attr;
    }

    public static ProductVariationAttribute CreateVariationAttribute(
        ProductVariation variation,
        ProductAttribute attribute,
        int id = 1,
        Action<ProductVariationAttribute>? overrideAction = null)
    {
        var vAttr = new ProductVariationAttribute
        {
            Id = id,
            Variation = variation,
            VariationId = variation.Id,
            Attribute = attribute,
            AttributeId = attribute.Id
        };

        overrideAction?.Invoke(vAttr);
        return vAttr;
    }

    // ----------------------------
    // SUBSCRIPTIONS
    // ----------------------------
    public static Subscription CreateSubscription(
        int id = 1,
        int productId = 1,
        SubscriptionFrequencyType freq = SubscriptionFrequencyType.Monthly,
        StatusType status = StatusType.Active,
        Action<Subscription>? overrideAction = null)
    {
        var subscription = new Subscription
        {
            Id = id,
            ProductId = productId,
            Frequency = freq,
            Status = status,
            StartDate = DateTime.UtcNow,
            PreviousBillingDate = DateTime.UtcNow.AddMonths(-1),
            NextBillingDate = DateTime.UtcNow.AddMonths(1),
            Product = new Product
            {
                Id = productId,
                Name = $"Product{productId}",
                Price = 10,
                ProductType = ProductType.Simple,
                Variations = [],
                GroupedProductItems = []
            }
        };

        overrideAction?.Invoke(subscription);
        return subscription;
    }

    public static UserSubscription CreateUserSubscription(
        int userId,
        int subscriptionId,
        int id = 1,
        Action<UserSubscription>? overrideAction = null)
    {
        var us = new UserSubscription
        {
            Id = id,
            UserId = userId,
            SubscriptionId = subscriptionId,
            User = new User
            {
                Id = userId,
                Email = $"user{userId}@test.com",
                PasswordHash = [0x01],
                PasswordSalt = [0x01]
            },
            Subscription = new Subscription
            {
                Id = subscriptionId,
                ProductId = 1,
                Frequency = SubscriptionFrequencyType.Monthly,
                Status = StatusType.Active,
                StartDate = DateTime.UtcNow,
                PreviousBillingDate = DateTime.UtcNow.AddMonths(-1),
                NextBillingDate = DateTime.UtcNow.AddMonths(1),
                Product = new Product
                {
                    Id = 1,
                    Name = "Product1",
                    Price = 10,
                    ProductType = ProductType.Simple,
                    Variations = [],
                    GroupedProductItems = []
                }
            }
        };

        overrideAction?.Invoke(us);
        return us;
    }
}