using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryWebApp.Models
{
    public class Book
    {
        [Key]
        public int Id { get; set; }

        public string Title { get; set; }

        [DisplayName("Publication Date")]
        public DateOnly PublicationDate { get; set; }

        [DisplayName("Available Count")]
        public int AvailableCount { get; set; }

        [DisplayName("Total Count")]
        public int TotalCount { get; set; }

        // TODO add book cover image support

        public ICollection<Genre>? Genres { get; set; }
        
        public ICollection<Author>? Authors { get; set; }

    }
}
