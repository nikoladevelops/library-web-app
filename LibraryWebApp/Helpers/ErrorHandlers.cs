using LibraryWebApp.ViewModels;

namespace LibraryWebApp.Helpers
{
    public class ErrorHandlers
    {
        public static ErrorViewModel NotFound(string itemNotFound)
        {
            return new ErrorViewModel { StatusCode = "404", Discription = $"Oops! The {itemNotFound} you're looking for can't be found." };
        }

        internal static ErrorViewModel AccessDenied()
        {
            return new ErrorViewModel { StatusCode = "403", Discription = "Uh-oh! You don’t have permission to view this page or resource." };
        }

        internal static ErrorViewModel NoAvailableBooks()
        {
            return new ErrorViewModel { StatusCode = "404", Discription = "Unfortunately there aren't any available copies of this book." };
        }

        internal static ErrorViewModel OverdueBooks()
        {
            return new ErrorViewModel { StatusCode = "403", Discription = "You must return your overdue books before renting new ones." };
        }
    }
}
