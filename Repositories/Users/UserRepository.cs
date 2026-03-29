using ECommerceProductsAPI.Data;
using ECommerceProductsAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace ECommerceProductsAPI.Repositories.Users;

public class UserRepository(ProductsDataContext context) : IUserRepository
{
    private readonly ProductsDataContext _context = context;

    public async Task<User?> GetUserDetailsById(int userId, bool asNoTracking = true)
    {
        var query = _context.Users
            .Include(u => u.Addresses)
            .Include(u => u.Subscriptions)
                .ThenInclude(s => s.Subscription)
                .ThenInclude(p => p.Product)
            .AsQueryable();

        if (asNoTracking)
        {
            query = query.AsNoTracking();
        }

        return await query.FirstOrDefaultAsync(u => u.Id == userId);
    }
}
