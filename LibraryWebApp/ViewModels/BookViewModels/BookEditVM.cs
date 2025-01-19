using System.ComponentModel;

namespace LibraryWebApp.ViewModels.BookViewModels
{
    public class BookEditVM : BookVM
    {
        [DisplayName("Available Count")]
        public int AvailableCount { get; set; }
    }
}
