using LibraryWebApp.Models;

namespace LibraryWebApp.ViewModels
{
    public class AuthorDetailsViewModel
    {
        public Author Author {  get; set; }
        public List<Book> Books { get; set; }
    }
}
