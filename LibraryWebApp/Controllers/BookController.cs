using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LibraryWebApp.Models;
using Microsoft.AspNetCore.Authorization;
using LibraryWebApp.Helpers;
using LibraryWebApp.ViewModels.BookViewModels;

namespace LibraryWebApp.Controllers
{
    public class BookController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IBookCoverImageManager _bookCoverImageManager;
        public BookController(ApplicationDbContext context, IBookCoverImageManager imageSaver)
        {
            _context = context;
            _bookCoverImageManager = imageSaver;
        }

        [Authorize(Roles = Globals.Roles.Admin)]
        public async Task<IActionResult> Index()
        {
            List<Book> allBooks = await _context.Books.ToListAsync();
            return View(allBooks);
        }

        public async Task<IActionResult> Details(int id)
        {
            var book = await _context.Books
            .Include(b => b.Authors)
            .Include(b => b.Genres)
            .FirstOrDefaultAsync(b => b.Id == id);

            if (book == null)
            {
                return View("Error",ErrorViewModelTypes.NotFound("book"));
            }

            var vm = new BookDetailsVM
            {
                Id = book.Id,
                Title = book.Title,
                PublicationDate = book.PublicationDate,
                TotalCount = book.TotalCount,
                AvailableCount = book.AvailableCount,
                CoverImageUrl = book.CoverImageUrl,
                DefaultCoverImageUrl = _bookCoverImageManager.DefaultNoBookCoverImagePath,
                SelectedAuthorIDs = book.Authors.Select(a => a.Id).ToList(),
                SelectedGenreIDs = book.Genres.Select(g => g.Id).ToList(),
                AvailableAuthors = new MultiSelectList(_context.Authors, "Id", "Name"),
                AvailableGenres = new MultiSelectList(_context.Genres, "Id", "Name"),
            };

            return View(vm);
        }


        [Authorize(Roles = Globals.Roles.Admin)]
        public IActionResult Create()
        {
            var vm = new BookCreateVM
            {
                AvailableAuthors = new MultiSelectList(_context.Authors, "Id", "Name"),
                AvailableGenres = new MultiSelectList(_context.Genres, "Id", "Name"),
                DefaultCoverImageUrl = _bookCoverImageManager.DefaultNoBookCoverImagePath,
            };

            return View(vm);
        }

        [Authorize(Roles = Globals.Roles.Admin)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BookCreateVM vm, IFormFile? coverImage)
        {
            ValidatePublicationDate(vm.PublicationDate);
            ValidateBookHasAuthors(vm.SelectedAuthorIDs);
            ValidateBookHasGenres(vm.SelectedGenreIDs);

            if (ModelState.IsValid)
            {
                var book = new Book
                {
                    Title = vm.Title,
                    TotalCount = vm.TotalCount,
                    AvailableCount = vm.TotalCount,
                    PublicationDate = vm.PublicationDate,
                    Authors = await _context.Authors
                        .Where(a => vm.SelectedAuthorIDs.Contains(a.Id))
                        .ToListAsync(),
                    Genres = await _context.Genres
                        .Where(g => vm.SelectedGenreIDs.Contains(g.Id))
                        .ToListAsync(),
                };

                // Save a brand new cover image if it was provided
                await SaveBookCoverImage(book, coverImage);

                _context.Books.Add(book);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            vm.AvailableAuthors = new MultiSelectList(_context.Authors, "Id", "Name");
            vm.AvailableGenres = new MultiSelectList(_context.Genres, "Id", "Name");
            
            return View(vm);
        }

        [Authorize(Roles = Globals.Roles.Admin)]
        public async Task<IActionResult> Edit(int id)
        {
            var book = await _context.Books
            .Include(b => b.Authors)
            .Include(b => b.Genres)
            .FirstOrDefaultAsync(b => b.Id == id);

            if (book == null)
                return View("Error",ErrorViewModelTypes.NotFound("book"));

            var vm = new BookEditVM
            {
                Id = book.Id,
                Title = book.Title,
                PublicationDate = book.PublicationDate,
                TotalCount = book.TotalCount,
                AvailableCount = book.AvailableCount,
                CoverImageUrl = book.CoverImageUrl,
                DefaultCoverImageUrl = _bookCoverImageManager.DefaultNoBookCoverImagePath,
                SelectedAuthorIDs = book.Authors.Select(a => a.Id).ToList(),
                SelectedGenreIDs = book.Genres.Select(g => g.Id).ToList(),
                AvailableAuthors = new MultiSelectList(_context.Authors, "Id", "Name"),
                AvailableGenres = new MultiSelectList(_context.Genres, "Id", "Name"),
            };

            return View(vm);
        }


        [Authorize(Roles = Globals.Roles.Admin)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(BookEditVM vm, IFormFile? coverImage)
        {
            var bookInDb = await _context.Books
                .Include(b => b.Authors)
                .Include(b => b.Genres)
                .FirstOrDefaultAsync(b => b.Id == vm.Id);

            if (bookInDb == null)
            {
                return View("Error",ErrorViewModelTypes.NotFound("book"));
            }

            ValidatePublicationDate(vm.PublicationDate);
            ValidateBookHasAuthors(vm.SelectedAuthorIDs);
            ValidateBookHasGenres(vm.SelectedGenreIDs);

            if (ModelState.IsValid)
            {

                bookInDb.Title = vm.Title;
                bookInDb.TotalCount = vm.TotalCount;
                bookInDb.AvailableCount = vm.AvailableCount;
                bookInDb.PublicationDate = vm.PublicationDate;

                // Always delete old cover image if it exists
                if (!string.IsNullOrEmpty(bookInDb.CoverImageUrl))

                // If the book had a cover image already saved in the database, delete it
                if (bookInDb.CoverImageUrl != null)
                {
                    _bookCoverImageManager.DeleteBookCoverImage(bookInDb.CoverImageUrl);
                }

                // Save a brand new cover image if it was provided
                await SaveBookCoverImage(bookInDb, coverImage);

                var selectedAuthors = await _context.Authors
                    .Where(a => vm.SelectedAuthorIDs.Contains(a.Id))
                    .ToListAsync();

                bookInDb.Authors = selectedAuthors;

                var selectedGenres = await _context.Genres
                    .Where(g => vm.SelectedGenreIDs.Contains(g.Id))
                    .ToListAsync();

                bookInDb.Genres = selectedGenres;

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            vm.AvailableAuthors = new MultiSelectList(_context.Authors, "Id", "Name");
            vm.AvailableGenres = new MultiSelectList(_context.Genres, "Id", "Name");

            return View(vm);
        }


        [Authorize(Roles = Globals.Roles.Admin)]
        public async Task<IActionResult> Delete(int id)
        {
            var book = await _context.Books
                .Include(b => b.Authors)
                .Include(b => b.Genres)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (book == null)
            {
                return View("Error", ErrorViewModelTypes.NotFound("book"));
            }

            BookDeleteVM vm = new BookDeleteVM
            {
                Id = book.Id,
                Title = book.Title,
                PublicationDate = book.PublicationDate,
                TotalCount = book.TotalCount,
                AvailableCount = book.AvailableCount,
                CoverImageUrl = book.CoverImageUrl,
                DefaultCoverImageUrl = _bookCoverImageManager.DefaultNoBookCoverImagePath,
                SelectedAuthorIDs = book.Authors.Select(a => a.Id).ToList(),
                SelectedGenreIDs = book.Genres.Select(g => g.Id).ToList(),
                AvailableAuthors = new MultiSelectList(_context.Authors, "Id", "Name"),
                AvailableGenres = new MultiSelectList(_context.Genres, "Id", "Name"),
            };

            return View(vm);
        }


        [Authorize(Roles = Globals.Roles.Admin)]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePOST(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book != null)
            {
                if (book.CoverImageUrl != null)
                {
                    _bookCoverImageManager.DeleteBookCoverImage(book.CoverImageUrl);
                }

                _context.Books.Remove(book);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Validates that the book has at least one author selected
        /// </summary>
        /// <param name="authorIDs"></param>
        private void ValidateBookHasAuthors(IEnumerable<int> authorIDs) 
        {
            if (authorIDs == null || !authorIDs.Any())
            {
                ModelState.AddModelError("SelectedAuthorIDs", "You must select at least one author.");
            }
        }

        /// <summary>
        /// Validates that the book has at least one genre selected
        /// </summary>
        /// <param name="genreIDs"></param>
        private void ValidateBookHasGenres(IEnumerable<int> genreIDs)
        {
            if (genreIDs == null || !genreIDs.Any())
            {
                ModelState.AddModelError("SelectedGenreIDs", "You must select at least one genre.");
            }
        }

        /// <summary>
        /// Validates publication date to ensure that it is not in the future
        /// </summary>
        /// <param name="publicationDate"></param>
        private void ValidatePublicationDate(DateOnly publicationDate)
        {
            if (publicationDate > DateOnly.FromDateTime(DateTime.Today))
            {
                ModelState.AddModelError("PublicationDate", "The publication date cannot be in the future.");
            }
        }

        /// <summary>
        /// Saves the book cover image to the disk if it was provided. Also sets the book's CoverImageUrl property to be consistent with that (either to null or to the actual book cover image url).
        /// </summary>
        /// <param name="book"></param>
        /// <param name="coverImage"></param>
        /// <returns></returns>
        private async Task SaveBookCoverImage(Book book, IFormFile? coverImage)
        {
            // If no cover image was actually passed, that means the book should also be consistent with that
            if (coverImage == null)
            {
                book.CoverImageUrl = null; // Important to ensure that the book has no cover image url if coverImage is null
                return;
            }

            // If a new book cover image was provided, then save it
            string filePath = await _bookCoverImageManager.SaveBookCoverImageToDiskAsync(coverImage);
            book.CoverImageUrl = filePath;
        }
    }
}
