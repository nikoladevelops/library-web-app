using Microsoft.AspNetCore.Identity;

namespace LibraryWebApp.Models
{
    public class ApplicationUser:IdentityUser
    {
        public ICollection<RentedBook>? RentedBooks { get; set; }
    }
}
