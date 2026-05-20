using System.ComponentModel.DataAnnotations;

namespace OfficeAssetManager.Core.DTO
{
    public class RegisterDto
    {
        [Required(ErrorMessage = "Username is required.")]
        [RegularExpression(@"^[a-zA-Z0-9]+$", ErrorMessage = "Username must contain only letters and numbers without spaces.")]
        public string UserName { get; set; } = null!;

        [Required(ErrorMessage = "Email address is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address format.")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Password is required.")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters long.")]
        // Matches the Identity requirements: Upper, lower, digit, symbol
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).+$",
            ErrorMessage = "Password must have at least one lowercase letter, one uppercase letter, one number, and one special character.")]
        public string Password { get; set; } = null!;
    }
}