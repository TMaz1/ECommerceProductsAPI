namespace ECommerceProductsAPI.Services.Users.Password;

public interface IPasswordService
{
    bool IsStrongPassword(string password);
    void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt);
    bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt);
}