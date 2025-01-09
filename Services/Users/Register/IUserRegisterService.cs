using ECommerceProductsAPI.Dtos;
using ECommerceProductsAPI.Dtos.Users;

namespace ECommerceProductsAPI.Services.Users.Register;

public interface IUserRegisterService
{
    Task<ServiceResponse<UserResponse>> Register(UserRegister userRequest);
    Task<ServiceResponse<UserResponse>> Login(UserLogin userLogin);
}