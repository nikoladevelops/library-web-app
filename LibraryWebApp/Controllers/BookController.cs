using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LibraryWebApp.Models;
using Microsoft.AspNetCore.Authorization;
using LibraryWebApp.ViewModels;
using LibraryWebApp.Helpers;
using static System.Runtime.InteropServices.JavaScript.JSType;

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

        public async Task<IActionResult> Index()
        {
            List<Book> allBooks = await _context.Books.ToListAsync();
            return View(allBooks);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Books.FindAsync(id);

            if (book == null)
            {
                return NotFound();
            }

            // TODO should use viewmodels instead
            ViewData["NoBookCoverImage"] = _bookCoverImageManager.DefaultNoBookCoverImagePath;

            return View(book);
        }

        public IActionResult Create()
        {
            return View(new BookCreateViewModel
            {
                AvailableAuthors = new MultiSelectList(_context.Authors, "Id", "Name"),
                AvailableGenres = new MultiSelectList(_context.Genres, "Id", "Name")
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BookCreateViewModel data, IFormFile? coverImage)
        {
            ValidatePublicationDate(data.PublicationDate);

            if (ModelState.IsValid)
            {
                Book book = new Book
                {
                    Title = data.Title,
                    AvailableCount = data.TotalCount,
                    TotalCount = data.TotalCount,
                    PublicationDate = data.PublicationDate,
                    Authors = _context.Authors.
                        Where(author => data.SelectedAuthorIDs.
                            Contains(author.Id)).
                        Include(author => author.Books).
                        ToList(),
                    Genres = _context.Genres.
                        Where(genre => data.SelectedGenreIDs.
                            Contains(genre.Id)).
                        Include(genre => genre.Books).
                        ToList(),
                };
                await SaveBookCoverImage(book, coverImage);

                foreach (var id in data.SelectedAuthorIDs)
                {
                    Author author = await _context.Authors.FindAsync(id);
                    author.Books.Add(book);
                    _context.Update(author);
                }
                foreach (var id in data.SelectedGenreIDs)
                {
                    Genre genre = await _context.Genres.FindAsync(id);
                    genre.Books.Add(book);
                    _context.Update(genre);
                }

                _context.Add(book);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(data);
        }
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Books.Include(book => book.Genres).Include(book => book.Authors).FirstOrDefaultAsync(book => book.Id == id);
            if (book == null)
            {
                return NotFound();
            }

            return View(new BookUpdateViewModel
            {
                AvailableAuthors = new MultiSelectList(_context.Authors.Where(author => !book.Authors.Contains(author)), "Id", "Name"),
                AvailableGenres = new MultiSelectList(_context.Genres.Where(genre => !book.Genres.Contains(genre)), "Id", "Name"),
                PrevSelectedAuthors = _context.Authors.Where(author => book.Authors.Contains(author)).ToList(),
                PrevSelectedGenres = _context.Genres.Where(genre => book.Genres.Contains(genre)).ToList(),
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(BookUpdateViewModel data, IFormFile? coverImage)
        {
            var bookInDb = await _context.Books.FindAsync(data.Id);

            if (bookInDb == null)
            {
                return NotFound();
            }

            ValidatePublicationDate(data.PublicationDate);

            if (ModelState.IsValid)
            {
                // If the book had a cover image already saved in the database, delete it
                if (bookInDb.CoverImageUrl != null)
                {
                    // Deletes the actual file
                    _bookCoverImageManager.DeleteBookCoverImage(bookInDb.CoverImageUrl);
                }

                // Now book has no bookInDbCoverImageUrl which is good.

                Book book = new Book
                {
                    Id = data.Id,
                    CoverImageUrl = data.CoverImageUrl,
                    Title = data.Title,
                    TotalCount = data.TotalCount,
                    AvailableCount = data.AvailableCount,
                    Authors = _context.Authors.
                        Where(author => data.SelectedAuthorIDs.
                            Contains(author.Id)).
                        Include(author => author.Books).
                        ToList(),
                    Genres = _context.Genres.
                        Where(genre => data.SelectedGenreIDs.
                            Contains(genre.Id)).
                        Include(genre => genre.Books).
                        ToList(),
                };

                _context.Entry(bookInDb).CurrentValues.SetValues(book); // This also copied the old book cover image url

                foreach (Author author in _context.Authors)
                {
                    if (author.Books == null) continue;

                    if (author.Books.Contains(bookInDb))
                    {
                        if (data.SelectedAuthorIDs.Contains(author.Id)) continue;
                        else
                        {
                            author.Books.Remove(bookInDb);
                        }
                    }
                    else
                    {
                        if (!data.SelectedAuthorIDs.Contains(author.Id)) continue;
                        else
                        {
                            author.Books.Add(bookInDb);
                        }
                    }
                }
                foreach (Genre genre in _context.Genres)
                {
                    if (genre.Books == null) continue;

                    if (genre.Books.Contains(bookInDb))
                    {
                        if (data.SelectedGenreIDs.Contains(genre.Id)) continue;
                        else
                        {
                            genre.Books.Remove(bookInDb);
                        }
                    }
                    else
                    {
                        if (!data.SelectedGenreIDs.Contains(genre.Id)) continue;
                        else
                        {
                            genre.Books.Add(bookInDb);
                        }
                    }
                }

                await SaveBookCoverImage(bookInDb, coverImage);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(data);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Books.FindAsync(id);

            if (book == null)
            {
                return NotFound();
            }

            // TODO should use viewmodels instead
            ViewData["NoBookCoverImage"] = _bookCoverImageManager.DefaultNoBookCoverImagePath;

            return View(book);
        }

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
