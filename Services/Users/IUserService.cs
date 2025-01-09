using ECommerceProductsAPI.Dtos.Users;
using ECommerceProductsAPI.Dtos;

namespace ECommerceProductsAPI.Services.Users;

public interface IUserService
{
    Task<ServiceResponse<List<UserResponse>>> GetAllUsers();
    Task<ServiceResponse<PaginatedResponse<UserResponse>>> GetUsersWithPagination(int pageNumber, int totalItemsPerPage);
    Task<ServiceResponse<UserResponse>> GetUserById(int id);
    Task<ServiceResponse<List<UserResponse>>> GetUserByFilter(FilterUsersRequest searchRequest);
    Task<ServiceResponse<PaginatedResponse<UserResponse>>> GetUsersByFilterWithPagination(int pageNumber, int totalItemsPerPage, FilterUsersRequest filterRequest);
    Task<ServiceResponse<UserResponse>> UpdateUser(int id, UserRequest userRequest);
    Task<ServiceResponse<bool>> UpdatePassword(int id, UpdatePasswordRequest passwordRequest);
    Task<ServiceResponse<string>> DeleteUser(int id);
}