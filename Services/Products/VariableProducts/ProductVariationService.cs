using ECommerceProductsAPI.Data;
using ECommerceProductsAPI.Dtos;
using ECommerceProductsAPI.Dtos.Products.Requests.Variations;
using ECommerceProductsAPI.Dtos.Products.Responses;
using ECommerceProductsAPI.Models;
using ECommerceProductsAPI.Models.Enums;
using ECommerceProductsAPI.Repositories.Products;
using ECommerceProductsAPI.Utils.Mappers;
using Microsoft.EntityFrameworkCore;

namespace ECommerceProductsAPI.Services.Products.VariableProducts;
public class ProductVariationService(ProductsDataContext context, IProductRepository productRepository) : IProductVariationService
{
    private readonly ProductsDataContext _context = context;
    private readonly IProductRepository _productRepository = productRepository;

    public async Task<ServiceResponse<ProductResponse>> AddProductVariation(AddVariationRequest request)
    {
        var response = new ServiceResponse<ProductResponse>();

        try
        {
            var product = await GetProductWithVariationsByProductId(request.ProductId) ?? throw new Exception($"Product with ID '{request.ProductId}' does not exist.");

            if (product.ProductType != ProductType.Variable)
            {
                throw new Exception($"Product with ID '{request.ProductId}' does not accept variations. To accept variations, change current product type from '{Enum.GetName(typeof(ProductType), product.ProductType!)}' to '{ProductType.Variable}'");
            }

            var updatedAttributes = await GetAndUpdateUniqueAttributes(request.Attributes);

            var productVariation = new ProductVariation
            {
                SKU = request.Variation.SKU,
                Price = request.Variation.Price,
                Stock = request.Variation.Stock,
                VariationAttributes = updatedAttributes.Select(attribute => new ProductVariationAttribute { AttributeId = attribute.Id }).ToList()
            };

            product.Variations.Add(productVariation);
            await _context.SaveChangesAsync();

            var updatedProduct = await _productRepository.GetProductDetailsById(request.ProductId) ?? throw new Exception($"Updated product with ID '{request.ProductId}' cannot be found.");

            response.Data = MapToProductResponse(updatedProduct);
            response.Message = $"Successfully added variation with ID '{productVariation.Id}' for product ID '{request.ProductId}'.";
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = $"Error creating product variation: {ex.Message}";
        }

        return response;
    }

    public async Task<ServiceResponse<ProductResponse>> UpdateProductVariation(UpdateVariationRequest request)
    {
        var response = new ServiceResponse<ProductResponse>();

        try
        {
            var product = await _productRepository.GetProductDetailsById(request.ProductId, asNoTracking: false) ?? throw new Exception($"Product with ID '{request.ProductId}' does not exist.");

            var existingVariation = product.Variations.FirstOrDefault(v => v.Id == request.VariationId) ?? throw new Exception($"Variation with ID {request.VariationId} does not exist in product ID {request.ProductId}.");

            // update current attribute by attribute id
            if (request.CurrentAttributes != null && request.CurrentAttributes.Count > 0)
            {
                await UpdateProductAttributes(existingVariation, request.CurrentAttributes);
            }

            // append new attributes to the existing variation
            if (request.NewAttributes != null && request.NewAttributes.Count > 0)
            {
                var updatedAttributes = await GetAndUpdateUniqueAttributes(request.NewAttributes);
                AppendAttributesToVariation(existingVariation, updatedAttributes);
            }


            // update variation fields only if request.Variation is provided
            if (request.Variation != null)
            {
                if (!string.IsNullOrWhiteSpace(request.Variation.SKU))
                {
                    existingVariation.SKU = request.Variation.SKU;
                }

                if (request.Variation.Price > 0)
                {
                    existingVariation.Price = request.Variation.Price;
                }

                if (request.Variation.Stock >= 0)
                {
                    existingVariation.Stock = request.Variation.Stock;
                }
            }

            await _context.SaveChangesAsync();

            response.Data = MapToProductResponse(product);
            response.Message = $"Successfully updated variation with ID '{request.VariationId}' for product ID '{request.ProductId}'.";
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = $"Error updating product variation with ID '{request.VariationId}': {ex.Message}";
        }

        return response;
    }

    public async Task<ServiceResponse<ProductResponse>> DeleteVariationByProductAndVariationId(int productId, int variationId)
    {
        var response = new ServiceResponse<ProductResponse>();

        try
        {
            // ensure variation belongs to the specified product id
            var variation = await _context.ProductVariations
                .Include(v => v.VariationAttributes)
                .FirstOrDefaultAsync(v => v.Id == variationId && v.ProductId == productId) ?? throw new Exception($"Variation with ID '{variationId}' does not exist in product ID '{productId}'.");

            // cascade delete
            _context.ProductVariations.Remove(variation);
            await _context.SaveChangesAsync();

            var updatedProduct = await _productRepository.GetProductDetailsById(productId) ?? throw new Exception($"Updated product with ID '{productId}' cannot be found.");

            response.Data = MapToProductResponse(updatedProduct);
            response.Message = $"Successfully deleted product variation with ID {variationId}.";
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = $"Error deleting product variation with ID '{variationId}': {ex.Message}";
        }

        return response;
    }

    public async Task<ServiceResponse<string>> CleanUpProductAttributes()
    {
        var response = new ServiceResponse<string>();

        try
        {
            // get all product attribute IDs that exists in ProductVariationAttributes
            var usedAttributeIds = await _context.ProductVariationAttributes
                .Select(pva => pva.AttributeId)
                .Distinct()
                .ToListAsync();

            // find the remaining product attributes that are not used by any product variation
            var unusedAttributes = await _context.ProductAttributes
                .Where(pa => !usedAttributeIds.Contains(pa.Id))
                .ToListAsync();

            if (unusedAttributes.Count > 0)
            {
                _context.ProductAttributes.RemoveRange(unusedAttributes);
                await _context.SaveChangesAsync();
            }

            response.Data = string.Empty;
            response.Message = $"Successfully cleaned up {unusedAttributes.Count} unused product attributes.";
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = $"Error cleaning up product attributes: {ex.Message}";
        }

        return response;
    }

    private async Task<List<ProductAttribute>> GetAndUpdateUniqueAttributes(List<AddProductAttributeRequest> requestAttributes)
    {
        var uniqueAttributes = new List<ProductAttribute>();
        var attributes = requestAttributes.Select(a => new { a.Type, a.Value }).ToList();
        var existingAttributes = await _context.ProductAttributes.Where(a => attributes.Select(k => k.Type).Contains(a.Type)).ToListAsync();

        foreach (var attribute in requestAttributes)
        {
            // get attribute from existing attributes list, create a new attribute if null
            var existingAttribute = existingAttributes.FirstOrDefault(a => a.Type == attribute.Type && a.Value == attribute.Value);

            if (existingAttribute == null)
            {
                var newAttribute = new ProductAttribute
                {
                    Type = attribute.Type,
                    Value = attribute.Value
                };

                _context.ProductAttributes.Add(newAttribute);
                uniqueAttributes.Add(newAttribute);
            }
            else
            {
                uniqueAttributes.Add(existingAttribute);
            }
        }

        await _context.SaveChangesAsync();
        return uniqueAttributes;
    }

    private static void AppendAttributesToVariation(ProductVariation variation, List<ProductAttribute> attributes)
    {
        variation.VariationAttributes ??= [];

        foreach (var attribute in attributes)
        {
            if (!variation.VariationAttributes.Any(va => va.AttributeId == attribute.Id))
            {
                variation.VariationAttributes.Add(new ProductVariationAttribute { AttributeId = attribute.Id });
            }
        }
    }

    private async Task UpdateProductAttributes(ProductVariation existingVariation, List<UpdateProductAttributeRequest> currentAttributes)
    {
        foreach (var currentAttribute in currentAttributes)
        {
            var existingAttribute = (existingVariation.VariationAttributes.FirstOrDefault(va => va.AttributeId == currentAttribute.AttributeId)?.Attribute) ?? throw new Exception($"Attribute with ID {currentAttribute.AttributeId} does not exist for current variation.");

            // detach attribute from product variation by deleting its instance in ProductVariationAttribute table
            var variationAttribute = existingVariation.VariationAttributes.FirstOrDefault(va => va.AttributeId == currentAttribute.AttributeId);

            if (variationAttribute != null)
            {
                existingVariation.VariationAttributes.Remove(variationAttribute);
                _context.ProductVariationAttributes.Remove(variationAttribute);
            }

            // find if updated attribute exists in attribute table and add it to the product variation
            var updatedAttribute = await _context.ProductAttributes.FirstOrDefaultAsync(a => a.Type == currentAttribute.Type && a.Value == currentAttribute.Value);

            // create new attribute if not found in table
            if (updatedAttribute == null)
            {
                updatedAttribute = new ProductAttribute
                {
                    Type = currentAttribute.Type ?? existingAttribute.Type,
                    Value = currentAttribute.Value ?? existingAttribute.Value
                };

                _context.ProductAttributes.Add(updatedAttribute);
                await _context.SaveChangesAsync();
            }

            // add the updated attribute to the variation
            existingVariation.VariationAttributes.Add(new ProductVariationAttribute
            {
                AttributeId = updatedAttribute.Id
            });


            // if no other variation uses the old attribute, delete attribute
            bool isAttributeUsed = await _context.ProductVariationAttributes.AnyAsync(va => va.AttributeId == currentAttribute.AttributeId);

            if (!isAttributeUsed)
            {
                var unusedAttribute = await _context.ProductAttributes.FirstOrDefaultAsync(a => a.Id == currentAttribute.AttributeId);

                if (unusedAttribute != null)
                {
                    _context.ProductAttributes.Remove(unusedAttribute);
                }
            }
        }

    }

    private async Task<Product?> GetProductWithVariationsByProductId(int productId)
    {
        return await _context.Products
            .Include(p => p.Variations)
                .ThenInclude(v => v.VariationAttributes)
                    .ThenInclude(va => va.Attribute)
            .FirstOrDefaultAsync(p => p.Id == productId);
    }


    private static ProductResponse MapToProductResponse(Product product) => ProductMapper.MapToProductResponse(product);
}