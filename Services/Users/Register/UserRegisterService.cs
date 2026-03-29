using ECommerceProductsAPI.Data;
using ECommerceProductsAPI.Dtos;
using ECommerceProductsAPI.Dtos.Users;
using ECommerceProductsAPI.Models;
using ECommerceProductsAPI.Repositories.Users;
using ECommerceProductsAPI.Services.Users.Password;
using ECommerceProductsAPI.Utils.Mappers;
using Microsoft.EntityFrameworkCore;

namespace ECommerceProductsAPI.Services.Users.Register;

public class UserRegisterService(ProductsDataContext context, IPasswordService passwordService, IUserRepository userRepository) : IUserRegisterService
{
    private readonly ProductsDataContext _context = context;
    private readonly IPasswordService _passwordService = passwordService;
    private readonly IUserRepository _userRepository = userRepository;

    public async Task<ServiceResponse<UserResponse>> Register(UserRegister userRequest)
    {
        var response = new ServiceResponse<UserResponse>();

        try
        {
            if (await CheckUserEmailExists(userRequest.Email))
            {
                throw new Exception("Register with another email.");
            }

            if (!_passwordService.IsStrongPassword(userRequest.Password))
            {
                throw new Exception("Password must be at least 8 characters, contain at least one uppercase letter, one lowercase letter, one number and one special character");
            }

            if (userRequest.Password != userRequest.ConfirmPassword)
            {
                throw new Exception("'Password' and 'Confirm Password' do not match");
            }


            _passwordService.CreatePasswordHash(userRequest.Password, out byte[] passwordHash, out byte[] passwordSalt);

            var user = new User
            {
                Email = userRequest.Email,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                FirstName = userRequest.FirstName,
                LastName = userRequest.LastName,
                PhoneNumber = userRequest.PhoneNumber,
            };

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            var addedUser = await _userRepository.GetUserDetailsById(user.Id) ?? throw new Exception("Failed to retrieve the newly added user.");

            response.Data = MapToUserResponse(addedUser);
            response.Message = $"Successfully registered new user with ID '{user.Id}'";
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = $"Error adding user: {ex.Message}";
        }

        return response;
    }

    public async Task<ServiceResponse<UserResponse>> Login(UserLogin userLogin)
    {
        var response = new ServiceResponse<UserResponse>();

        try
        {
            // get user by email
            User? user = await _context.Users
                .Where(u => string.Equals(u.Email, userLogin.Email))
                .FirstOrDefaultAsync();

            if (user == null || !_passwordService.VerifyPasswordHash(userLogin.Password, user.PasswordHash, user.PasswordSalt))
            {
                throw new Exception("Incorrect email or password.");
            }

            response.Data = MapToUserResponse(user);
            response.Message = $"Successfully logged user '{user.Id}' in";
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = $"Error retrieving user: {ex.Message}";
        }

        return response;
    }

    private async Task<bool> CheckUserEmailExists(string email)
    {
        return await _context.Users
            .AsNoTracking()
            .AnyAsync(u => u.Email == email);
    }

    private static UserResponse MapToUserResponse(User user) => UserMapper.MapToUserResponse(user);
}