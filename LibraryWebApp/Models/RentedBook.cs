using System.ComponentModel.DataAnnotations;

namespace LibraryWebApp.Models
{
    public class RentedBook
    {
        [Key]
        public int Id { get; set; }
        public int BookId { get; set; }
        public Book Book { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        public DateOnly RentalDate { get; set; }
        public DateOnly? ReturnDate { get; set; }

        public Boolean IsReturned { get; set; }
    }
}
