using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryWebApp.Models
{
    public class Book
    {
        [Key]
        public int Id { get; set; }

        public string Title { get; set; }

        public DateTime PublicationDate { get; set; }

        public int AvailableCount { get; set; }

        public int TotalCount { get; set; }

        // TODO add book cover image support

        public ICollection<Genre>? Genres { get; set; }
        // Made them nullable for the time being so i can add books to the database
        // Fix this in the future 
        public ICollection<Author>? Authors { get; set; }

    }
}
