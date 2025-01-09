
using ECommerceProductsAPI.Dtos;
using ECommerceProductsAPI.Dtos.Products.Requests;
using ECommerceProductsAPI.Dtos.Products.Responses;

namespace ECommerceProductsAPI.Services.Products;

public interface IProductService
{
    Task<ServiceResponse<List<ProductResponse>>> GetAllProducts();
    Task<ServiceResponse<PaginatedResponse<ProductResponse>>> GetProductsWithPagination(int pageNumber, int totalItemsPerPage);
    Task<ServiceResponse<ProductResponse>> GetProductById(int id);
    Task<ServiceResponse<List<ProductResponse>>> GetProductsByFilter(FilterProductsRequest queryFilter);
    Task<ServiceResponse<PaginatedResponse<ProductResponse>>> GetProductsByFilterWithPagination(int pageNumber, int totalItemsPerPage, FilterProductsRequest queryFilter);
    Task<ServiceResponse<ProductResponse>> AddProduct(AddProductRequest productRequest);
    Task<ServiceResponse<ProductResponse>> UpdateProduct(int id, UpdateProductRequest updatedProduct);
    Task<ServiceResponse<string>> DeleteProduct(int id);
    Task<ServiceResponse<bool>> IsProductExistsById(int id);
}