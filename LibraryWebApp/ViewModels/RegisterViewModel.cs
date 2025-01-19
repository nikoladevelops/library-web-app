using System.ComponentModel.DataAnnotations;

namespace LibraryWebApp.ViewModels
{
    public class RegisterViewModel
    {

        public string Username { get; set; }
        [EmailAddress]
        public string Email { get; set; }

        [RegularExpression(@"^(?=.*[A-Z])(?=.*[a-z])(?=.*\d)(?=.*[\W_]).*$", ErrorMessage = "Password must have at least one uppercase letter, one lowercase letter, one digit, and one special character.")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters long.")]
        [DataType(DataType.Password)]
        [Compare("ConfirmPassword", ErrorMessage = "Password must match")]
        public string Password { get; set; }
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }
        [DataType(DataType.PhoneNumber)]
        public string PhoneNumber { get; set; }
    }
}
