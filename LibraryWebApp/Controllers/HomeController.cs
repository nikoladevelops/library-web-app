using LibraryWebApp.Helpers;
using LibraryWebApp.Models;
using LibraryWebApp.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Linq;

namespace LibraryWebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IBookCoverImageManager _bookCoverImageManager;

        private int booksPerPage = 10;

        public HomeController(ApplicationDbContext context, ILogger<HomeController> logger, UserManager<ApplicationUser> userManager, IBookCoverImageManager bookCoverImageManager)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
            _bookCoverImageManager = bookCoverImageManager;
        }

        public enum SearchBy
        {
            Title,
            Author,
            Genre
        }

        public enum OrderBy
        {
            None,
            MostPopular,
            LeastPopular,
            RecentlyPublished,
            OldestPublished
        }

        public async Task<IActionResult> Index(
            int? page = 1,
            string? searchTerm = null,
            SearchBy? searchBy = null,
            OrderBy? orderBy = null
            )
        {
            // Any page number less than 1 is invalid, so set it to 1
            if (page < 1)
            {
                page = 1;
            }

            IQueryable<Book> books = _context.Books;

            // Filter by search term
            if (searchTerm != null && searchBy != null)
            {
                searchTerm = searchTerm.Trim();

                switch (searchBy)
                {
                    case SearchBy.Title:
                        books = books.Where(b => b.Title.Contains(searchTerm));
                        break;
                    case SearchBy.Author:
                        books = books
                        .Include(b => b.Authors)
                        .Where(b => b.Authors.Any(a => a.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)));
                        break;
                    case SearchBy.Genre:
                        books = books
                       .Include(b => b.Genres)
                       .Where(b => b.Genres.Any(g => g.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)));
                        break;
                }
            }

            // Max total pages of books after all filtering was done
            int maxPageCount = (int)Math.Ceiling((double)books.Count() / booksPerPage);

            // Skip the books that are on previous pages
            books = books.Skip((page.Value - 1) * booksPerPage);

            // Take the books only for the current page, ignore the rest
            books = books.Take(booksPerPage);


            // Order the books as the user desires
            if (orderBy != null)
            {
                switch (orderBy)
                {
                    case OrderBy.MostPopular:
                        books = books.Include(b => b.RentedBooks)
                            .OrderByDescending(b => b.RentedBooks.Count()); 
                        break;
                    case OrderBy.LeastPopular:
                        books = books.Include(b => b.RentedBooks)
                            .OrderBy(b => b.RentedBooks.Count());
                        break;
                    case OrderBy.RecentlyPublished:
                        books = books.OrderByDescending(b => b.PublicationDate);
                        break;
                    case OrderBy.OldestPublished:
                        books = books.OrderBy(b => b.PublicationDate);
                        break;
                }
            }

            var bookData = books.Select(b => new BookSimplified
            {
                Id = b.Id,
                Title = b.Title,
                CoverImageUrl = b.CoverImageUrl != null ? b.CoverImageUrl : _bookCoverImageManager.DefaultNoBookCoverImagePath
            }).ToList();

            var searchByDropDown = new List<SelectListItem>
            {
                new SelectListItem { Text = SearchBy.Title.ToString(), Value = SearchBy.Title.ToString() },
                new SelectListItem { Text = SearchBy.Author.ToString(), Value = SearchBy.Author.ToString() },
                new SelectListItem { Text = SearchBy.Genre.ToString(), Value = SearchBy.Genre.ToString() }
            };

            var orderByDropDown = new List<SelectListItem>
            {
                new SelectListItem { Text = OrderBy.None.ToString(), Value = OrderBy.None.ToString() },
                new SelectListItem { Text = "Most Popular", Value = OrderBy.MostPopular.ToString() },
                new SelectListItem { Text = "Least Popular", Value = OrderBy.LeastPopular.ToString() },
                new SelectListItem { Text = "Recently Published", Value = OrderBy.RecentlyPublished.ToString() },
                new SelectListItem { Text = "Oldest Published", Value = OrderBy.OldestPublished.ToString() }
            };

            var viewModel = new HomeViewModel
            {
                BookData = bookData,
                Page = page.Value,
                MaxPage = maxPageCount,
                SearchTerm = searchTerm,
                SearchByDropDown = searchByDropDown,
                SearchBy = searchBy?.ToString(),
                OrderByDropDown = orderByDropDown,
                OrderBy = orderBy?.ToString()
            };

            return View(viewModel);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View();
        }
    }
}
