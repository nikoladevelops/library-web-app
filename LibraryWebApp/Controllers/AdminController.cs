using LibraryWebApp.Helpers;
using LibraryWebApp.Models;
using LibraryWebApp.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol.Plugins;
using Sprache;
using static LibraryWebApp.Controllers.HomeController;

namespace LibraryWebApp.Controllers
{
    public class AdminController : Controller
    {

        private readonly ApplicationDbContext _context;
        private readonly IBookCoverImageManager _bookCoverImageManager;


        public AdminController(ApplicationDbContext con, IBookCoverImageManager img)
        {
            _context = con;
            _bookCoverImageManager = img;
        }
        public async Task<IActionResult> Panel(AdminPanelViewModel? vm)
        {
            return View(vm);
        }
        public async Task<IActionResult> TopBooks()
        {
            IQueryable<Book> books = _context.Books;
            books = books.Include(b => b.RentedBooks).OrderByDescending(b => b.RentedBooks.Count()).Take(10);
            var bookData = books.Select(b => new BookSimplified
            {
                Id = b.Id,
                Title = b.Title,
                CoverImageUrl = b.CoverImageUrl != null ? b.CoverImageUrl : _bookCoverImageManager.DefaultNoBookCoverImagePath
            }).ToList();

            var viewModel = new AdminPanelViewModel
            {
                TopBooks = bookData
            };
            return View("Panel", viewModel);

        }
        public async Task<IActionResult> TopUsers()
        {
            IQueryable<ApplicationUser> users = _context.Users;
            users = users.Include(b => b.RentedBooks).OrderByDescending(b => b.RentedBooks.Count()).Take(10);
            var viewModel = new AdminPanelViewModel
            {
                TopUsers = users.ToList()
            };
             return View("Panel", viewModel);
        }
        public async Task<IActionResult> GetAllUsers()
        {
            IQueryable<ApplicationUser> users = _context.Users;
            var actives = GetActiveUsers();
            var inactives = GetInactiveusers();
            var viewModel = new AdminPanelViewModel
            {
                InactiveUsers = inactives.ToList(),
                ActiveUsers = actives.ToList(),
            };
            return View("Panel", viewModel);
        }
        public IQueryable<ApplicationUser> GetActiveUsers()
        {
            var activeUsers = _context.Users
            .Where(user => _context.RentedBooks
            .Where(rentedBook => rentedBook.UserId == user.Id && rentedBook.ReturnedAt == null)
            .Any()) 
            .Distinct(); 
            return activeUsers;
        }
        public IQueryable<ApplicationUser> GetInactiveusers()
        {
            var inactiveUsers = _context.Users
              .Where(user => !_context.RentedBooks
              .Any(rentedBook => rentedBook.UserId == user.Id && rentedBook.ReturnedAt == null))
              .Distinct();
            return inactiveUsers;
        }
        public async Task<IActionResult> Details(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            ApplicationUser user = await _context.Users.FindAsync(id);
            return View(user);
        }
        public async Task<IActionResult> Delete(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeletePOST(string id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Panel));
        }
        [Authorize(Roles = Globals.Roles.Admin)]
        public async Task<IActionResult> Edit(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }
        [Authorize(Roles = Globals.Roles.Admin)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ApplicationUser u)
        {
            var user = await _context.Users.FindAsync(u.Id);

            if (ModelState.IsValid)
            {
                _context.Entry(user).CurrentValues.SetValues(u); 
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Panel));
            }

            return View(user);
        }

        public async Task<IActionResult> ReturnAll(string id) 
        {
                var rentedBooks = await _context.RentedBooks
                                   .Where(b => b.UserId == id)
                                   .ToListAsync();
            foreach (var rentedBook in rentedBooks)
            {
                if (rentedBook.ReturnedAt == null)
                {
                    var book = await _context.Books.FindAsync(rentedBook.BookId);
                    book.AvailableCount += 1;
                    rentedBook.ReturnedAt = DateOnly.FromDateTime(DateTime.Now);
                }
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Panel));

        }
        public async Task<IActionResult> ReturnLate(string id)
        {
            var rentedBooks = await _context.RentedBooks
                                  .Where(b => b.UserId == id)
                                  .ToListAsync();
            foreach (var rentedBook in rentedBooks)
            {

                if (rentedBook.Deadline < DateOnly.FromDateTime(DateTime.Now) && rentedBook.ReturnedAt == null)
                { 
                    var book = await _context.Books.FindAsync(rentedBook.BookId);
                    book.AvailableCount += 1;
                    rentedBook.ReturnedAt = DateOnly.FromDateTime(DateTime.Now);
                }
            }
            await _context.SaveChangesAsync(); 
            return RedirectToAction(nameof(Panel));
        }
        public async Task<IActionResult> BanAndUnbanUser(string id)
        {
            var user = await _context.Users.FindAsync(id);
            if (!user.IsBanned) user.IsBanned = true;
            else user.IsBanned = false;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Panel));
        }
    }
}
