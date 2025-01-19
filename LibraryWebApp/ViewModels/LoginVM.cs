using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace LibraryWebApp.ViewModels
{
    public class LoginVM
    {
        [StringLength(30)]
        [DisplayName("Username")]
        public string Username { get; set; }

        [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters long.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

    }
}
