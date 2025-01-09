using ECommerceProductsAPI.Data;
using ECommerceProductsAPI.Dtos;
using ECommerceProductsAPI.Dtos.Users;
using ECommerceProductsAPI.Models;
using ECommerceProductsAPI.Repositories;
using ECommerceProductsAPI.Services.Users.Password;
using ECommerceProductsAPI.Utils;
using Microsoft.EntityFrameworkCore;

namespace ECommerceProductsAPI.Services.Users;
public class UserService(ProductsDataContext context, IPasswordService passwordService, UserRepository userRepository) : IUserService
{
    private readonly ProductsDataContext _context = context;
    private readonly IPasswordService _passwordService = passwordService;
    private readonly UserRepository _userRepository = userRepository;

    public async Task<ServiceResponse<List<UserResponse>>> GetAllUsers()
    {
        var response = new ServiceResponse<List<UserResponse>>();
        try
        {
            var users = await _context.Users
                .AsNoTracking()
                .Include(u => u.Addresses)
                .Include(u => u.Subscriptions)
                    .ThenInclude(u => u.Subscription)
                    .ThenInclude(p => p.Product)
                .Select(u => MapToUserResponse(u))
                .AsQueryable()
                .ToListAsync();

            response.Data = users;
            response.Message = $"Successfully retrieved all {users.Count} users";
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = $"Error retrieving users: {ex.Message}";
        }

        return response;
    }

    public async Task<ServiceResponse<PaginatedResponse<UserResponse>>> GetUsersWithPagination(int pageNumber, int totalItemsPerPage)
    {
        var response = new ServiceResponse<PaginatedResponse<UserResponse>>();
        try
        {
            if (totalItemsPerPage <= 0 || pageNumber <= 0)
            {
                throw new Exception("Invalid number. Please enter a number greater than 0.");
            }

            // calculate how many pages via product total and size of page
            int totalNumberOfUsers = await _context.Users.CountAsync();
            int totalNumberOfPages = (int)Math.Ceiling((double)totalNumberOfUsers / totalItemsPerPage);

            if (pageNumber > totalNumberOfPages)
            {
                throw new Exception($"Invalid page number. Page {pageNumber} does not exist. There are only {totalNumberOfPages} pages.");
            }

            var users = await _context.Users
                .Include(u => u.Addresses)
                .Include(u => u.Subscriptions)
                    .ThenInclude(u => u.Subscription)
                    .ThenInclude(p => p.Product)
                .AsNoTracking()
                .Skip((pageNumber - 1) * totalItemsPerPage)
                .Take(totalItemsPerPage)
                .Select(p => MapToUserResponse(p))
                .ToListAsync();


            response.Data = new PaginatedResponse<UserResponse>
            {
                Items = users,
                CurrentPageNumber = pageNumber,
                TotalPages = totalNumberOfPages,
                ItemsPerPage = totalItemsPerPage,
                TotalItems = totalNumberOfUsers,
            };

            response.Message = $"Successfully retrieved {users.Count} users.";
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = $"Error retrieving products: {ex.Message}";
        }

        return response;
    }

    public async Task<ServiceResponse<UserResponse>> GetUserById(int id)
    {
        var response = new ServiceResponse<UserResponse>();

        try
        {
            var user = await _userRepository.GetUserDetailsByIdNoTracking(id) ?? throw new Exception($"User with ID '{id}' not found.");

            response.Data = MapToUserResponse(user);
            response.Message = $"Successfully retrieved user with ID '{id}'";
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = $"Error retrieving user: {ex.Message}";
        }

        return response;
    }

    public async Task<ServiceResponse<List<UserResponse>>> GetUserByFilter(FilterUsersRequest filterRequest)
    {
        var response = new ServiceResponse<List<UserResponse>>();

        try
        {
            var users = await BuildUserQuery(filterRequest)
                .Select(u => MapToUserResponse(u))
                .ToListAsync();

            response.Data = users;
            response.Message = $"Successfully retrieved {users.Count} users.";
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = $"Error retrieving users: {ex.Message}";
        }

        return response;
    }

    public async Task<ServiceResponse<PaginatedResponse<UserResponse>>> GetUsersByFilterWithPagination(int pageNumber, int totalItemsPerPage, FilterUsersRequest filterRequest)
    {
        var response = new ServiceResponse<PaginatedResponse<UserResponse>>();

        try
        {
            if (totalItemsPerPage <= 0 || pageNumber <= 0)
            {
                throw new Exception("Invalid number. Please enter a number greater than 0.");
            }

            int totalNumberOfUsers = await BuildUserQuery(filterRequest).CountAsync();
            int totalNumberOfPages = (int)Math.Ceiling((double)totalNumberOfUsers / totalItemsPerPage);

            if (totalNumberOfUsers == 0 || totalNumberOfPages == 0)
            {
                throw new Exception("No results found.");
            }

            if (pageNumber > totalNumberOfPages)
            {
                throw new Exception($"Invalid page number. There are only {totalNumberOfPages} page(s).");
            }

            var users = await BuildUserQuery(filterRequest)
                .Skip((pageNumber - 1) * totalItemsPerPage)
                .Take(totalItemsPerPage)
                .Select(p => MapToUserResponse(p))
                .ToListAsync();

            response.Data = new PaginatedResponse<UserResponse>
            {
                Items = users,
                CurrentPageNumber = pageNumber,
                TotalPages = totalNumberOfPages,
                ItemsPerPage = totalItemsPerPage,
                TotalItems = totalNumberOfUsers,
            };

            response.Message = $"Successfully retrieved {users.Count} users.";
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = $"Error retrieving user(s): {ex.Message}";
        }

        return response;
    }


    public async Task<ServiceResponse<UserResponse>> UpdateUser(int id, UserRequest updatedUser)
    {
        var response = new ServiceResponse<UserResponse>();

        try
        {
            var user = await _userRepository.GetUserDetailsByIdWithTracking(id) ?? throw new Exception($"User with ID '{id}' not found.");

            if (user.Email != updatedUser.Email && !string.IsNullOrWhiteSpace(updatedUser.Email))
            {
                user.Email = updatedUser.Email;
            }

            if (user.FirstName != updatedUser.FirstName && !string.IsNullOrWhiteSpace(updatedUser.FirstName))
            {
                user.FirstName = updatedUser.FirstName;
            }

            if (user.LastName != updatedUser.LastName && !string.IsNullOrWhiteSpace(updatedUser.LastName))
            {
                user.LastName = updatedUser.LastName;
            }

            if (user.PhoneNumber != updatedUser.PhoneNumber && !string.IsNullOrWhiteSpace(updatedUser.PhoneNumber))
            {
                user.PhoneNumber = updatedUser.PhoneNumber;
            }

            user.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            response.Data = MapToUserResponse(user);
            response.Message = $"Successfully updated user with ID '{id}'";
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = $"Error updating user: {ex.Message}";
        }

        return response;
    }

    public async Task<ServiceResponse<bool>> UpdatePassword(int id, UpdatePasswordRequest passwordRequest)
    {
        var response = new ServiceResponse<bool>();

        try
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id) ?? throw new Exception($"User with ID '{id}' not found.");

            // verify current password
            if (!_passwordService.VerifyPasswordHash(passwordRequest.CurrentPassword, user.PasswordHash, user.PasswordSalt))
            {
                throw new Exception("Current password is incorrect.");
            }

            if (!_passwordService.IsStrongPassword(passwordRequest.NewPassword))
            {
                throw new Exception("New password must be at least 8 characters, contain at least one uppercase letter, one lowercase letter, one number and one special character");
            }

            // hash and update new password
            _passwordService.CreatePasswordHash(passwordRequest.NewPassword, out byte[] passwordHash, out byte[] passwordSalt);

            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;
            user.UpdatedAt = DateTime.UtcNow;

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            response.Data = true;
            response.Message = $"Successfully updated password for user with ID '{id}'.";
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = $"Error updating password: {ex.Message}";
        }

        return response;
    }

    public async Task<ServiceResponse<string>> DeleteUser(int id)
    {
        var response = new ServiceResponse<string>();

        try
        {
            var user = await _userRepository.GetUserDetailsByIdWithTracking(id) ?? throw new Exception($"User with ID '{id}' not found.");

            // get all subscription IDs from user's subscriptions and remove related subscriptions from the Subscription table
            var subscriptionIds = user.Subscriptions.Select(us => us.SubscriptionId).ToList();

            var subscriptionsToRemove = await _context.Subscriptions.Where(s => subscriptionIds.Contains(s.Id)).ToListAsync();

            if (subscriptionsToRemove.Count > 0)
            {
                _context.Subscriptions.RemoveRange(subscriptionsToRemove);
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            response.Data = string.Empty;
            response.Message = $"Successfully deleted user with ID '{id}'";
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = $"Error deleting user: {ex.Message}";
        }
        return response;
    }


    private IQueryable<User> BuildUserQuery(FilterUsersRequest filter)
    {
        var query = _context.Users.AsNoTracking();

        if (!string.IsNullOrEmpty(filter.Email))
        {
            query = query.Where(u => u.Email != null && u.Email.Contains(filter.Email));
        }

        if (!string.IsNullOrEmpty(filter.FirstName))
        {
            query = query.Where(u => u.FirstName != null && u.FirstName.Contains(filter.FirstName));
        }

        if (!string.IsNullOrEmpty(filter.LastName))
        {
            query = query.Where(u => u.LastName != null && u.LastName.Contains(filter.LastName));
        }

        if (!string.IsNullOrEmpty(filter.PhoneNumber))
        {
            query = query.Where(u => u.PhoneNumber != null && u.PhoneNumber.Contains(filter.PhoneNumber));
        }

        if (filter.CreatedAfter.HasValue)
        {
            query = query.Where(u => u.CreatedAt >= filter.CreatedAfter.Value);
        }

        if (filter.CreatedBefore.HasValue)
        {
            query = query.Where(u => u.CreatedAt <= filter.CreatedBefore.Value);
        }

        if (filter.HasSubscriptions.HasValue)
        {
            query = query.Where(u => u.Subscriptions != null && u.Subscriptions.Any());
        }

        query = query.Include(u => u.Addresses)
                     .Include(u => u.Subscriptions)
                        .ThenInclude(us => us.Subscription);

        return query;
    }

    private static UserResponse MapToUserResponse(User user) => UserMapper.MapToUserResponse(user);
}