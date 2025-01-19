using System.ComponentModel;

namespace LibraryWebApp.ViewModels.BookViewModels
{
    public class BookEditVM : BookVM
    {
        [DisplayName("Available Count")]
        public int AvailableCount { get; set; }

        public bool IsBookCoverChanged { get; set; } = false; // Needed in order to handle cases where the user does not want to override the book cover image
    }
}
