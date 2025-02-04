﻿using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryWebApp.Models
{
    public class Book
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

        public ICollection<Genre> Genres { get; set; }
        
        public ICollection<Author> Authors { get; set; }

        public ICollection<RentedBook>? RentedBooks { get; set; }
    }
}
