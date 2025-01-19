using System.ComponentModel;

namespace LibraryWebApp.ViewModels.BookViewModels
{
    public class BookDetailsVM : BookVM
    {
        [DisplayName("Available Count")]
        public int AvailableCount { get; set; }

        [DisplayName("All Authors")]
        public string AllAuthorsJoinedString { get; set; }

        [DisplayName("All Genres")]
        public string AllGenresJoinedString { get; set; }
    }
}
