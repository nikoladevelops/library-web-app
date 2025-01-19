using System.ComponentModel;

namespace LibraryWebApp.ViewModels.BookViewModels
{
    public class BookDetailsVM : BookVM
    {
        [DisplayName("Available Count")]
        public int AvailableCount { get; set; }
    }
}
