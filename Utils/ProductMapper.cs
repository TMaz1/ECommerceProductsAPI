using ECommerceProductsAPI.Dtos.Products.Responses;
using ECommerceProductsAPI.Models;
using ECommerceProductsAPI.Models.Enums;

namespace ECommerceProductsAPI.Utils;

public static class ProductMapper
{
    public static BasicProductResponse MapToBasicProductResponse(Product product)
    {
        if (product == null)
        {
            throw new ArgumentNullException(nameof(product), "Product cannot be null.");
        }

        return new BasicProductResponse
        {
            Id = product.Id,
            Name = product.Name,
            ShortDescription = product.ShortDescription,
            ProductType = Enum.GetName(typeof(ProductType), product.ProductType!) ?? "Unknown",
            Price = product.Price,
        };
    }

    public static ProductResponse MapToProductResponse(Product product)
    {
        return new ProductResponse
        {
            Id = product.Id,
            Name = product.Name,
            ShortDescription = product.ShortDescription,
            Description = product.Description,
            Price = product.Price,
            ProductType = Enum.GetName(typeof(ProductType), product.ProductType!) ?? "Unknown",
            IsSubscription = product.IsSubscription,
            CreatedAt = product.CreatedAt,
            LastUpdated = product.UpdatedAt,
            Variations = product.Variations.Select(variation => new ProductVariationResponse
            {
                Id = variation.Id,
                SKU = variation.SKU,
                Price = variation.Price,
                Stock = variation.Stock,
                Attributes = variation.VariationAttributes.Select(va => new ProductAttributeResponse
                {
                    Id = va.Attribute.Id,
                    Type = va.Attribute.Type,
                    Value = va.Attribute.Value
                }).ToList()
            }).ToList(),
            GroupedProductItems = product.GroupedProductItems?.Select(gpi => new GroupedProductItemResponse
            {
                Id = gpi.Id,
                Quantity = gpi.Quantity,
                ProductDetails = new BasicProductResponse
                {
                    Id = gpi.ProductItem.Id,
                    Name = gpi.ProductItem.Name,
                    ShortDescription = gpi.ProductItem.ShortDescription,
                    Price = gpi.ProductItem.Price,
                    ProductType = gpi.ProductItem.ProductType.ToString() ?? "",
                    IsSubscription = gpi.ProductItem.IsSubscription
                }
            }).ToList() ?? []
        };
    }
}