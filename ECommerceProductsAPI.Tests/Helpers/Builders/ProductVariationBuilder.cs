using ECommerceProductsAPI.Models;

namespace ECommerceProductsAPI.Tests.Helpers.Builders;

public class ProductVariationBuilder
{
    private readonly ProductVariation _variation = new()
    {
        SKU = "SKU-001",
        Price = 9.99m,
        Stock = 100,
        VariationAttributes = []
    };

    public ProductVariationBuilder ForProduct(Product product)
    {
        _variation.Product = product;
        _variation.ProductId = product.Id;
        return this;
    }

    public ProductVariationBuilder WithSKU(string sku)
    {
        _variation.SKU = sku;
        return this;
    }

    public ProductVariationBuilder WithPrice(decimal price)
    {
        _variation.Price = price;
        return this;
    }

    public ProductVariationBuilder WithStock(int stock)
    {
        _variation.Stock = stock;
        return this;
    }

    public ProductVariationBuilder AddVariationAttribute(ProductVariationAttribute attr)
    {
        _variation.VariationAttributes.Add(attr);
        return this;
    }

    public ProductVariation Build() => _variation;
}