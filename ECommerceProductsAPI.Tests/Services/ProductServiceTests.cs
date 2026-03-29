using Xunit;
using ECommerceProductsAPI.Services.Products;
using ECommerceProductsAPI.Repositories.Products;
using ECommerceProductsAPI.Tests.Helpers;

namespace ECommerceProductsAPI.Tests.Services;

public class ProductServiceTests
{
    [Fact]
    public async Task GetAllProducts_ReturnsAllProducts()
    {
        using var factory = new InMemoryContextFactory();

        // Seed data
        factory.Context.Products.AddRange(TestData.Products);
        await factory.Context.SaveChangesAsync();

        var productRepository = new ProductRepository(factory.Context); // real repo
        var service = new ProductService(factory.Context, productRepository);

        // Act
        var result = await service.GetAllProducts();

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.Equal(TestData.Products.Count, result.Data.Count);
        Assert.Contains(result.Data, p => p.Name == "Product A");
    }
}