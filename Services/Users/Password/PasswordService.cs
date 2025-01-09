using System.Security.Cryptography;
using System.Text;
using ECommerceProductsAPI.Utils.CustomRegex;

namespace ECommerceProductsAPI.Services.Users.Password;

public class PasswordService(IStrongPasswordRegex passwordRegex) : IPasswordService
{
    private readonly IStrongPasswordRegex _passwordRegex = passwordRegex;

    public bool IsStrongPassword(string password) => _passwordRegex.GetStrongPasswordRegex().IsMatch(password);
    
    public void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
    {
        using var hmac = new HMACSHA512();
        passwordSalt = hmac.Key;
        passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
    }

    public bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
    {
        using var hmac = new HMACSHA512(passwordSalt);
        var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        return computedHash.SequenceEqual(passwordHash);
    }
}
