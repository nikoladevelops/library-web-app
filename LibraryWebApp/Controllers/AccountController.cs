using LibraryWebApp.Models;
using LibraryWebApp.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryWebApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationDbContext _context;

        public AccountController( SignInManager<ApplicationUser> signIn, UserManager<ApplicationUser> um, ApplicationDbContext con)
        {
            _signInManager = signIn;
            _userManager = um;
            _context = con;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var res = await _signInManager.PasswordSignInAsync(model.Username, model.Password, false, false);

                if (res.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel registerViewModel)
        {
            if (ModelState.IsValid)
            {
                // Check if there is already a user with this username
                var existingUserByUsername = await _userManager.FindByNameAsync(registerViewModel.Username);
                if (existingUserByUsername != null)
                {
                    ModelState.AddModelError("Username", "A user with this username already exists.");
                    return View(registerViewModel);
                }

                // Check if there is already a user with this email
                var existingUserByEmail = await _userManager.FindByEmailAsync(registerViewModel.Email);
                if (existingUserByEmail != null)
                {
                    ModelState.AddModelError("Email", "A user with this email address already exists.");
                    return View(registerViewModel);
                }

                // Create a new user
                ApplicationUser user = new ApplicationUser
                {
                    UserName = registerViewModel.Username,
                    Email = registerViewModel.Email
                };

                var result = await _userManager.CreateAsync(user, registerViewModel.Password);
                await _userManager.AddToRoleAsync(user, "User");

                if (result.Succeeded)
                {
                    await _signInManager.PasswordSignInAsync(user.UserName, registerViewModel.Password, false, false);
                    return RedirectToAction("Index", "Home");
                }

                // Add errors to ModelState if user creation failed
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(registerViewModel);
        }
        public async Task<IActionResult> Rent(int bookId, string userId)
        {
            Book? book = await _context.Books.FindAsync(bookId);
            List<RentedBook> userRentedBooks = await _context.RentedBooks.Where(r => r.UserId == userId).ToListAsync();
            if (book == null) 
            {
                return NotFound();
            }

            if (book.AvailableCount == 0)
            {
                // TODO maybe a better custom error view - the book cannot be rented, no available copies
                // or maybe a ModelState error and redirect to the book details page?

                return NotFound();
            }
            if (HasOverdueBooks(userId, userRentedBooks))
            {
                // TODO add an error page or redirect to proper place, due to user having unreturned books that are overdue
                return NotFound();
            }

            book.AvailableCount -= 1;

            DateOnly returnDate = today.AddDays(Globals.BookRentDayLimit);

            //finding if user has any overdue books

            var entry = new RentedBook()
            {
                UserId = userId,
                BookId = bookId,
                RentalDate = today,
                ReturnDate = returnDate
            };
            _context.RentedBooks.Add(entry);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Home");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Return(int Id)
        {
            var rBook = await _context.RentedBooks.FindAsync(Id);
            var book = await _context.Books.FindAsync(rBook.BookId);
            if (rBook != null)
            {
                book.AvailableCount += 1;
                _context.Books.Update(book);
                rBook.ReturnedAt = DateOnly.FromDateTime(DateTime.Now);
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Details));
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(Index)); 
        } 
        public async Task<IActionResult> Details()
        {
            var currentUserId = _userManager.GetUserId(User);

            var rentedBooks = await _context.RentedBooks
               .Include(a => a.Book)  
               .Include(a => a.User)  
               .Where(a => a.UserId == currentUserId) 
               .ToListAsync();

            return View(rentedBooks);
        }
        private bool HasOverdueBooks(string userId, List<RentedBook> books)
        {
            foreach (var item in books)
            {
                if (item.ReturnedAt == null && item.ReturnDate < DateOnly.FromDateTime(DateTime.Now))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
