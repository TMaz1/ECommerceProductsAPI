using ECommerceProductsAPI.Data;
using ECommerceProductsAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace ECommerceProductsAPI.Repositories.Products;

public class ProductRepository(ProductsDataContext context) : IProductRepository
{
    private readonly ProductsDataContext _context = context;

    public async Task<Product?> GetProductDetailsById(int productId, bool asNoTracking = true)
    {
        var query = _context.Products
            .Include(p => p.Variations)
                .ThenInclude(v => v.VariationAttributes)
                    .ThenInclude(va => va.Attribute)
            .Include(p => p.GroupedProductItems)
                .ThenInclude(gpi => gpi.ProductItem)
            .AsQueryable();

        if (asNoTracking)
        {
            query = query.AsNoTracking();
        }

        return await query.FirstOrDefaultAsync(p => p.Id == productId);
    }
}