using System.ComponentModel.DataAnnotations;

namespace ECommerceProductsAPI.Dtos.Users;

public class UserRegister
    {
        [Required(ErrorMessage="Please fill the '{0}' field"), EmailAddress]
        public string Email { get; set; } = string.Empty;
    
        [Required(ErrorMessage="Please fill the '{0}' field"), RegularExpression(@"^(?=(.*\d))(?=.*[a-z])(?=.*[A-Z])(?=.*[^a-zA-Z\d]).{8,}$", ErrorMessage="'Password' must be at least 8 characters, contain at least one uppercase letter, one lowercase letter, one number and one special character")]
        public string Password { get; set; } = string.Empty;
        
        [Required(ErrorMessage="Please fill the 'Confirm Password' field"), Compare("Password", ErrorMessage="'Password' and 'Confirm Password' do not match")]
        public string ConfirmPassword { get; set; } = string.Empty;
        
        [Required(ErrorMessage="Please fill the 'First Name' field"), MinLength(3, ErrorMessage="{0} must be at least 3 characters")]
        public string FirstName { get; set; } = string.Empty;
        
        [Required(ErrorMessage="Please fill the '{0}' field"), MinLength(3, ErrorMessage="{0} must be at least 3 characters")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please fill the '{0}' field"), RegularExpression(@"^\+?[1-9]\d{1,14}$", ErrorMessage = "Invalid Phone Number field")]
        public string PhoneNumber { get; set; } = string.Empty;
    }