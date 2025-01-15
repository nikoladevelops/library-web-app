using LibraryWebApp.Models;
using LibraryWebApp.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace LibraryWebApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly ApplicationDbContext context;

        public AccountController( SignInManager<ApplicationUser> signIn, UserManager<ApplicationUser> um, ApplicationDbContext con)
        {
            signInManager = signIn;
            userManager = um;
            context = con;
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
                var res = await signInManager.PasswordSignInAsync(model.Username, model.Password, false, false);

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
                var existingUserByUsername = await userManager.FindByNameAsync(registerViewModel.Username);
                if (existingUserByUsername != null)
                {
                    ModelState.AddModelError("Username", "A user with this username already exists.");
                    return View(registerViewModel);
                }

                // Check if there is already a user with this email
                var existingUserByEmail = await userManager.FindByEmailAsync(registerViewModel.Email);
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

                var result = await userManager.CreateAsync(user, registerViewModel.Password);
                await userManager.AddToRoleAsync(user, "User");

                if (result.Succeeded)
                {
                    await signInManager.PasswordSignInAsync(user.UserName, registerViewModel.Password, false, false);
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

        public async Task<IActionResult> Rent(int bookId, string userId, DateOnly returnDate)
        {
            RentedBook entry = new();
            var book =  context.Books.FirstOrDefault(b => b.Id == bookId);
            if (book.TotalCount > 0)
            {
                book.TotalCount -= 1;

                context.SaveChanges();
                entry = new()
                {
                    UserId = userId,
                    BookId = bookId,
                    RentalDate = DateOnly.FromDateTime(DateTime.Now),
                    ReturnDate = returnDate
                };
            }
            context.RentedBooks.Add(entry);
            await context.SaveChangesAsync();

            return RedirectToAction("Index", "Home");
        }
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction(nameof(Index)); 
        }
        public async Task<IActionResult> Details()
        {
            List<RentedBook> rented = await context.RentedBooks.ToListAsync();
            foreach (var item in rented)
            {
              item.Book =  context.Books.FirstOrDefault(b => b.Id == item.BookId);
              item.User = await context.Users.FirstOrDefaultAsync(u => u.Id == item.UserId);
            }
            return View(rented);
        }
    }
}
