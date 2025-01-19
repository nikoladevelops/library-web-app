﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace LibraryWebApp.ViewModels.BookViewModels
{
    public class BookCreateVM : BookVM
    {
        [DisplayName("Authors")]
        public IEnumerable<int> SelectedAuthorIDs { get; set; }

        public MultiSelectList? AvailableAuthors { get; set; }

        [DisplayName("Genres")]
        public IEnumerable<int> SelectedGenreIDs { get; set; }

        public MultiSelectList? AvailableGenres { get; set; }
    }
}
