using ECommerceProductsAPI.Dtos.Products.Responses;
using ECommerceProductsAPI.Dtos.Products.Requests.Variations;
using ECommerceProductsAPI.Dtos;

namespace ECommerceProductsAPI.Services.Products.VariableProducts;

public interface IProductVariationService
{
    Task<ServiceResponse<ProductResponse>> AddProductVariation(AddVariationRequest request);
    Task<ServiceResponse<ProductResponse>> UpdateProductVariation(UpdateVariationRequest request);
    Task<ServiceResponse<ProductResponse>> DeleteVariationByProductAndVariationId(int productId, int variationId);
    Task<ServiceResponse<string>> CleanUpProductAttributes();
}