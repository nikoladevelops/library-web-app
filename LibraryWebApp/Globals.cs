namespace LibraryWebApp
{
    /// <summary>
    /// Used for constants and global variables
    /// </summary>
    public static class Globals
    {
        /// <summary>
        /// All user roles that the application supports as enum
        /// </summary>
        public enum AllRoles
        {
            Admin,
            User
        }

        /// <summary>
        /// All user roles that the application supports as string
        /// </summary>
        public static class Roles
        {
            public const string Admin = nameof(AllRoles.Admin);
            public const string User = nameof(AllRoles.User);
        }

        /// <summary>
        /// The amount of days the user can keep a book before it is considered overdue
        /// </summary>
        public static int BookRentDayLimit { get => 14; }

        /// <summary>
        /// The default date format used in the application
        /// </summary>
        public static string DateFormat { get => "dd/MM/yyyy"; }

        /// <summary>
        /// The number of days before the book return deadline when a warning message is displayed to the user.
        /// </summary>
        public static int RentDeadlineWarningPeriod { get => 3; }

    }
}
