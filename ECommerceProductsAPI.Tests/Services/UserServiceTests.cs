using Xunit;
using ECommerceProductsAPI.Services.Users;
using ECommerceProductsAPI.Repositories.Users;
using ECommerceProductsAPI.Services.Users.Password;
using ECommerceProductsAPI.Tests.Helpers;
using Moq;
using ECommerceProductsAPI.Utils.CustomRegex;

namespace ECommerceProductsAPI.Tests.Services;

public class UserServiceTests
{
    [Fact]
    public async Task GetAllUsers_ReturnsAllUsers()
    {
        using var factory = new InMemoryContextFactory();

        // Seed data
        factory.Context.Users.AddRange(TestData.Users);
        await factory.Context.SaveChangesAsync();

        var userRepository = new UserRepository(factory.Context);

        // used moq - not focused on password logic
        var mockRegex = new Mock<IStrongPasswordRegex>();
        var passwordService = new PasswordService(mockRegex.Object);

        var service = new UserService(factory.Context, passwordService, userRepository);

        // Act
        var result = await service.GetAllUsers();

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.Equal(TestData.Users.Count, result.Data.Count);

        // Check that a specific seeded user is returned
        Assert.Contains(result.Data, u => u.Email == "alice@example.com");
        Assert.Contains(result.Data, u => u.Email == "bob@example.com");

        // Optionally check nested addresses
        var alice = result.Data.First(u => u.Email == "alice@example.com");
        Assert.NotEmpty(alice.Addresses);
        Assert.Equal("123 Main St", alice.Addresses.First().StreetAddress);
    }
}