using ECommerceProductsAPI.Data;
using ECommerceProductsAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace ECommerceProductsAPI.Repositories;

public class UserRepository(ProductsDataContext context)
{
    private readonly ProductsDataContext _context = context;

    public async Task<User?> GetUserDetailsByIdNoTracking(int userId)
    {
        return await _context.Users
            .AsNoTracking()
            .Include(u => u.Addresses)
            .Include(u => u.Subscriptions)
                .ThenInclude(s => s.Subscription)
                .ThenInclude(p => p.Product)
            .FirstOrDefaultAsync(u => u.Id == userId);
    }

    public async Task<User?> GetUserDetailsByIdWithTracking(int userId)
    {
        return await _context.Users
            .Include(u => u.Addresses)
            .Include(u => u.Subscriptions)
                .ThenInclude(s => s.Subscription)
                .ThenInclude(p => p.Product)
            .FirstOrDefaultAsync(u => u.Id == userId);
    }

    public async Task<Product?> GetProductDetailsByIdNoTracking(int productId)
    {
        return await _context.Products
            .AsNoTracking()
            .Include(p => p.Variations)
                .ThenInclude(v => v.VariationAttributes)
                    .ThenInclude(va => va.Attribute)
            .Include(p => p.GroupedProductItems)
                .ThenInclude(gpi => gpi.ProductItem)
            .FirstOrDefaultAsync(p => p.Id == productId);
    }

    public async Task<Product?> GetProductDetailsByIdWithTracking(int productId)
    {
        return await _context.Products
            .Include(p => p.Variations)
                .ThenInclude(v => v.VariationAttributes)
                    .ThenInclude(va => va.Attribute)
            .Include(p => p.GroupedProductItems)
                .ThenInclude(gpi => gpi.ProductItem)
            .FirstOrDefaultAsync(p => p.Id == productId);
    }
}
