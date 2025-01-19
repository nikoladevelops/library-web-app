using LibraryWebApp.Models;

namespace LibraryWebApp.ViewModels
{
    public class AdminPanelViewModel
    {
        public IEnumerable<BookSimplified>? TopBooks { get; set; }
        public IEnumerable<ApplicationUser>? TopUsers { get; set; }
        public IEnumerable<ApplicationUser>? ActiveUsers { get; set; }
        public IEnumerable<ApplicationUser>? InactiveUsers { get; set; }
             
    }
}
