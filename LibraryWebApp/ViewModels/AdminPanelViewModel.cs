using LibraryWebApp.Models;

namespace LibraryWebApp.ViewModels
{
    public class AdminPanelViewModel
    {
        public List<BookSimplified>? TopBooks { get; set; }
        public List<ApplicationUser>? TopUsers { get; set; }
        public List<ApplicationUser>? ActiveUsers { get; set; }
        public List<ApplicationUser>? InactiveUsers { get; set; }
             
    }
}
