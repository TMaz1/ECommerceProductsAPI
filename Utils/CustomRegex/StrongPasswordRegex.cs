using System.Text.RegularExpressions;

namespace ECommerceProductsAPI.Utils.CustomRegex;

public interface IStrongPasswordRegex
{
    Regex GetStrongPasswordRegex();
}

public partial class StrongPasswordRegex : IStrongPasswordRegex
{
    // atleast 8 characters long, contains at least 1 uppercase, lowercase, digit and special character
    private const string PasswordPattern = @"^(?=(.*\d))(?=.*[a-z])(?=.*[A-Z])(?=.*[^a-zA-Z\d]).{8,}$";

    [GeneratedRegex(PasswordPattern)]
    private static partial Regex StrongPassword();

    public Regex GetStrongPasswordRegex() => StrongPassword();
}