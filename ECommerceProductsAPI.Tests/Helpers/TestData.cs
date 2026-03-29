using ECommerceProductsAPI.Models;
using ECommerceProductsAPI.Models.Enums;
using ECommerceProductsAPI.Tests.Helpers.Builders;

namespace ECommerceProductsAPI.Tests.Helpers;

public static class TestData
{
    public static List<User> Users
    {
        get
        {
            var user1 = new UserBuilder()
                .WithEmail("alice@example.com")
                .WithFirstName("Alice")
                .WithLastName("Smith")
                .AddAddress(new Address
                {
                    StreetAddress = "123 Main St",
                    City = "Wonderland",
                    Country = "Fantasy",
                    PostalCode = "00001"
                })
                .Build();

            var user2 = new UserBuilder()
                .WithEmail("bob@example.com")
                .WithFirstName("Bob")
                .WithLastName("Jones")
                .Build();

            return [user1, user2];
        }
    }


    public static List<Product> Products
    {
        get
        {
            var productA = new ProductBuilder()
                .WithName("Product A")
                .WithPrice(10)
                .WithProductType(ProductType.Simple)
                .AddVariation(new ProductVariationBuilder()
                    .WithSKU("A1")
                    .WithPrice(10)
                    .WithStock(50)
                    .Build())
                .Build();

            var productB = new ProductBuilder()
                .WithName("Product B")
                .WithPrice(20)
                .WithProductType(ProductType.Simple)
                .WithSubscription(true)
                .Build();

            return [productA, productB];
        }
    }
}