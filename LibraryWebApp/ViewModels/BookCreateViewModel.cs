using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace LibraryWebApp.ViewModels
{
    public class BookCreateViewModel
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
    }
}
