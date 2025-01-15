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

        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Book book, IFormFile? coverImage) // TODO should probably switch to using ViewModels
        {
            ValidatePublicationDate(book.PublicationDate);

            if (ModelState.IsValid)
            {
                await SaveBookCoverImage(book, coverImage);

                book.AvailableCount = book.TotalCount;
                _context.Add(book);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(book);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
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

            return View(book);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Book book, IFormFile? coverImage)
        {
            var bookInDb = await _context.Books.FindAsync(book.Id);

            if (bookInDb == null)
            {
                return NotFound();
            }


            ValidatePublicationDate(book.PublicationDate);

            if (ModelState.IsValid)
            {
                // If the book had a cover image already saved in the database, delete it
                if (bookInDb.CoverImageUrl != null)
                {
                    // Deletes the actual file
                    _bookCoverImageManager.DeleteBookCoverImage(bookInDb.CoverImageUrl);
                }

                // Now book has no bookInDbCoverImageUrl which is good.

                _context.Entry(bookInDb).CurrentValues.SetValues(book); // This also copied the old book cover image url

                await SaveBookCoverImage(bookInDb, coverImage);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(book);
        }

        [Authorize(Roles = "Admin")]
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

        [Authorize(Roles = "Admin")]
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
