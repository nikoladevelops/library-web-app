using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel;

namespace LibraryWebApp.ViewModels.BookViewModels
{
    public class BookDeleteVM : BookVM
    {
        [DisplayName("Available Count")]
        public int AvailableCount { get; set; }


        [DisplayName("Authors")]
        public IEnumerable<int> SelectedAuthorIDs { get; set; }

        public MultiSelectList? AvailableAuthors { get; set; }

        [DisplayName("Genres")]
        public IEnumerable<int> SelectedGenreIDs { get; set; }

        public MultiSelectList? AvailableGenres { get; set; }
    }
}
