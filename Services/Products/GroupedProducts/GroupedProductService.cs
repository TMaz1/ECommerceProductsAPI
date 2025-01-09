using ECommerceProductsAPI.Data;
using ECommerceProductsAPI.Dtos;
using ECommerceProductsAPI.Dtos.Products.Requests;
using ECommerceProductsAPI.Dtos.Products.Responses;
using ECommerceProductsAPI.Models;
using ECommerceProductsAPI.Models.Enums;
using ECommerceProductsAPI.Repositories;
using ECommerceProductsAPI.Utils;
using Microsoft.EntityFrameworkCore;

namespace ECommerceProductsAPI.Services.Products.GroupedProducts;

public class GroupedProductService(ProductsDataContext context, UserRepository userRepository) : IGroupedProductService
{
    private readonly ProductsDataContext _context = context;
    private readonly UserRepository _userRepository = userRepository;

    public async Task<ServiceResponse<ProductResponse>> AddProductToGroupedProduct(GroupedProductRequest request)
    {
        var response = new ServiceResponse<ProductResponse>();
        try
        {
            var groupedProduct = await GetGroupedProductById(request.GroupedProductId) ?? throw new Exception($"Grouped Product with ID '{request.GroupedProductId}' not found.");

            if (groupedProduct.ProductType != ProductType.Grouped)
            {
                throw new Exception($"Product with ID '{request.GroupedProductId}' is not a grouped product.");
            }

            if (request.ProductItemId == request.GroupedProductId)
            {
                throw new Exception($"Cannot add the grouped product to itself.");
            }

            var productItem = await _context.Products.FirstOrDefaultAsync(p => p.Id == request.ProductItemId) ?? throw new Exception($"Product with ID '{request.ProductItemId}' not found.");

            if (productItem.ProductType == ProductType.Grouped)
            {
                throw new Exception($"Product with ID '{request.ProductItemId}' is a grouped product. Cannot add a grouped product to another grouped product.");
            }

            if (groupedProduct.GroupedProductItems.Any(item => item.ProductItemId == request.ProductItemId))
            {
                throw new Exception($"Product with ID '{request.ProductItemId}' is already part of the grouped product.");
            }

            var groupedProductItem = new GroupedProductItem
            {
                GroupedProductId = groupedProduct.Id,
                ProductItemId = productItem.Id,
                Quantity = request.Quantity
            };

            groupedProduct.GroupedProductItems.Add(groupedProductItem);
            await _context.SaveChangesAsync();

            var updatedGroupedProduct = await _userRepository.GetProductDetailsByIdNoTracking(request.GroupedProductId) ?? throw new Exception($"Updated grouped product with ID '{request.GroupedProductId}' not retrieved.");

            response.Data = MapToProductResponse(updatedGroupedProduct);
            response.Message = $"Successfully added product with ID '{request.ProductItemId}' to the grouped product with ID '{request.GroupedProductId}'.";
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = $"Error adding product to grouped product: {ex.Message}";
        }
        return response;
    }

    public async Task<ServiceResponse<ProductResponse>> UpdateProductQuantityInGroupedProduct(GroupedProductRequest request)
    {
        var response = new ServiceResponse<ProductResponse>();
        try
        {
            var groupedProduct = await _userRepository.GetProductDetailsByIdWithTracking(request.GroupedProductId) ?? throw new Exception($"Grouped Product with ID '{request.GroupedProductId}' not found.");
            
            if (groupedProduct.ProductType != ProductType.Grouped)
            {
                throw new Exception($"Product with ID '{request.GroupedProductId}' is not a grouped product.");
            }

            var groupedProductItem = groupedProduct.GroupedProductItems.FirstOrDefault(gpi => gpi.ProductItemId == request.ProductItemId) ?? throw new Exception($"Product with ID '{request.ProductItemId}' is not part of the grouped product {request.GroupedProductId}.");

            if (request.Quantity >= 0)
            {
                groupedProductItem.Quantity = request.Quantity;
            }
            else
            {
                throw new Exception($"Invalid quantity.");
            }

            await _context.SaveChangesAsync();
            
            response.Data = MapToProductResponse(groupedProduct);
            response.Message = $"Successfully updated quantity for product with ID '{request.ProductItemId}' in grouped product with ID '{request.GroupedProductId}'.";
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = $"Error updating grouped product quantity: {ex.Message}";
        }
        return response;
    }


    public async Task<ServiceResponse<ProductResponse>> DeleteProductFromGroupedProduct(int groupedProductId, int productId)
    {
        var response = new ServiceResponse<ProductResponse>();
        try
        {
            var groupedProduct = await GetGroupedProductById(groupedProductId) ?? throw new Exception($"Grouped product with ID '{groupedProductId}' not found.");

            var groupedProductItem = groupedProduct.GroupedProductItems.FirstOrDefault(gpi => gpi.ProductItemId == productId) ?? throw new Exception($"Product with ID '{productId}' is not part of the grouped product with ID '{groupedProductId}'.");

            groupedProduct.GroupedProductItems.Remove(groupedProductItem);
            _context.GroupedProductItems.Remove(groupedProductItem);

            await _context.SaveChangesAsync();

            var updatedGroupedProduct = await _userRepository.GetProductDetailsByIdNoTracking(groupedProductId) ?? throw new Exception($"Updated product with ID '{productId}' could not be retrieved.");

            response.Data = MapToProductResponse(updatedGroupedProduct);
            response.Message = $"Successfully removed product with ID '{productId}' from grouped product with ID '{groupedProductId}'.";
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = $"Error deleting product with ID '{productId}' from grouped product with ID '{groupedProductId}': {ex.Message}";
        }
        return response;
    }

    private async Task<Product?> GetGroupedProductById(int productId)
    {
        return await _context.Products
                .Include(p => p.GroupedProductItems)
                    .ThenInclude(gpi => gpi.ProductItem)
                .FirstOrDefaultAsync(p => p.Id == productId);
    }

    private static ProductResponse MapToProductResponse(Product product) => ProductMapper.MapToProductResponse(product);
}