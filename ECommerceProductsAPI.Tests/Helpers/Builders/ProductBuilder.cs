using ECommerceProductsAPI.Models;
using ECommerceProductsAPI.Models.Enums;

namespace ECommerceProductsAPI.Tests.Helpers.Builders;

public class ProductBuilder
{
    private readonly Product _product = new()
    {
        Name = "Test Product",
        Price = 9.99m,
        ProductType = ProductType.Simple,
        IsSubscription = false,
        Variations = [],
        GroupedProductItems = []
    };

    public ProductBuilder WithName(string name)
    {
        _product.Name = name;
        return this;
    }

    public ProductBuilder WithPrice(decimal price)
    {
        _product.Price = price;
        return this;
    }

    public ProductBuilder WithProductType(ProductType type)
    {
        _product.ProductType = type;
        return this;
    }

    public ProductBuilder WithSubscription(bool isSubscription)
    {
        _product.IsSubscription = isSubscription;
        return this;
    }

    public ProductBuilder AddVariation(ProductVariation variation)
    {
        _product.Variations.Add(variation);
        return this;
    }

    public ProductBuilder AddGroupedProductItem(GroupedProductItem item)
    {
        _product.GroupedProductItems.Add(item);
        return this;
    }

    public Product Build() => _product;
}