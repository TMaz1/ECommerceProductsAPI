using ECommerceProductsAPI.Dtos;
using ECommerceProductsAPI.Dtos.Products.Requests;
using ECommerceProductsAPI.Dtos.Products.Responses;

namespace ECommerceProductsAPI.Services.Products.GroupedProducts;

public interface IGroupedProductService
{
    Task<ServiceResponse<ProductResponse>> AddProductToGroupedProduct(GroupedProductRequest request);
    Task<ServiceResponse<ProductResponse>> UpdateProductQuantityInGroupedProduct(GroupedProductRequest request);
    Task<ServiceResponse<ProductResponse>> DeleteProductFromGroupedProduct(int groupedProductId, int productId);
}