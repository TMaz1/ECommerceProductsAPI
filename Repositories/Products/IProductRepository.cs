using ECommerceProductsAPI.Models;

namespace ECommerceProductsAPI.Repositories.Products;

public interface IProductRepository
{
    Task<Product?> GetProductDetailsById(int productId, bool asNoTracking = true);
}