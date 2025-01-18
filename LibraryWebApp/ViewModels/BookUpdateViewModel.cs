﻿using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using LibraryWebApp.Models;

namespace LibraryWebApp.ViewModels
{
    public class BookUpdateViewModel
    {
        [Key]
        public int Id { get; set; }
        [RegularExpression(@"^[^\/\\:\*\<>\|]+$", ErrorMessage = "The Title contains invalid characters.")]
        public string Title { get; set; }

        [DisplayName("Publication Date")]
        public DateOnly PublicationDate { get; set; }

        [DisplayName("Available Count")]
        public int AvailableCount { get; set; }

        [DisplayName("Total Count")]
        public int TotalCount { get; set; }

        [DisplayName("Cover Image")]
        public string? CoverImageUrl { get; set; }

        [DisplayName("Authors")]
        public List<int> SelectedAuthorIDs { get; set; }
        public MultiSelectList? AvailableAuthors { get; set; }
        public List<Author>? PrevSelectedAuthors { get; set; }

        [DisplayName("Genres")]
        public List<int> SelectedGenreIDs { get; set; }
        public MultiSelectList? AvailableGenres { get; set; }
        public List<Genre>? PrevSelectedGenres { get; set; }
    }
}
