using Microsoft.AspNetCore.Identity;

namespace LibraryWebApp.Models
{
    public class ApplicationUser:IdentityUser
    {
        public ICollection<RentedBook>? RentedBooks { get; set; }
        public bool IsBanned { get; set; }
        public string? PhoneNumber { get; set; }
    }
}
