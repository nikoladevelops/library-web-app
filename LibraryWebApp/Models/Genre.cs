using System.ComponentModel.DataAnnotations;

namespace LibraryWebApp.Models
{
    public class Genre
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

        public ICollection<Book>? Books { get; set; }

    }
}
