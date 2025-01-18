using LibraryWebApp.Models;

namespace LibraryWebApp.ViewModels
{
    public class BookSimplified
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string? CoverImageUrl { get; set; }
    }
}
