using System.ComponentModel;

namespace LibraryWebApp.ViewModels.BookViewModels
{
    public class BookDeleteVM : BookVM
    {
        [DisplayName("Available Count")]
        public int AvailableCount { get; set; }
    }
}
