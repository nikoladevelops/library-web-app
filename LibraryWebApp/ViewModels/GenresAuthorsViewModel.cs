using LibraryWebApp.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace LibraryWebApp.ViewModels
{
    public class GenresAuthorsViewModel
    {
        public IEnumerable<SelectListItem> Authors { get; set; }
        public IEnumerable<SelectListItem> Genres { get; set; }
    }
}
