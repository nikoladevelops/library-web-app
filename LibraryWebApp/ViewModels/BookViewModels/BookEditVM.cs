using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel;

namespace LibraryWebApp.ViewModels.BookViewModels
{
    public class BookEditVM : BookVM
    {
        [DisplayName("Available Count")]
        public int AvailableCount { get; set; }

        public bool IsBookCoverChanged { get; set; } = false; // Needed in order to handle cases where the user does not want to override the book cover image

        [DisplayName("Authors")]
        public IEnumerable<int> SelectedAuthorIDs { get; set; }

        public MultiSelectList? AvailableAuthors { get; set; }

        [DisplayName("Genres")]
        public IEnumerable<int> SelectedGenreIDs { get; set; }

        public MultiSelectList? AvailableGenres { get; set; }
    }
}
