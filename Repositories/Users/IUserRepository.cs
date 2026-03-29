using ECommerceProductsAPI.Models;

namespace ECommerceProductsAPI.Repositories.Users;

public interface IUserRepository
{
    Task<User?> GetUserDetailsById(int userId, bool asNoTracking = true);
}