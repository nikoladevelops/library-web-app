using LibraryWebApp.Models;

namespace LibraryWebApp.ViewModels
{
    public class AuthorDetailsVM
    {
        public Author Author {  get; set; }
        public IEnumerable<Book> Books { get; set; }
    }
}
