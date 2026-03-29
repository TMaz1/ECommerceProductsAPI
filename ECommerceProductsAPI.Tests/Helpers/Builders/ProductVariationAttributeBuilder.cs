using ECommerceProductsAPI.Models;

namespace ECommerceProductsAPI.Tests.Helpers.Builders;

public class ProductVariationAttributeBuilder
{
    private readonly ProductVariationAttribute _attr = new();

    public ProductVariationAttributeBuilder ForVariation(ProductVariation variation)
    {
        _attr.Variation = variation;
        _attr.VariationId = variation.Id;
        return this;
    }

    public ProductVariationAttributeBuilder WithAttribute(ProductAttribute attribute)
    {
        _attr.Attribute = attribute;
        _attr.AttributeId = attribute.Id;
        return this;
    }

    public ProductVariationAttribute Build() => _attr;
}