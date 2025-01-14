namespace LibraryWebApp.Helpers
{
    public interface IBookCoverImageManager
    {
        public string DefaultNoBookCoverImagePath { get; }
        Task<string> SaveBookCoverImageToDiskAsync(IFormFile coverImage);

        void DeleteBookCoverImage(string coverImageUrl);
    }
}
