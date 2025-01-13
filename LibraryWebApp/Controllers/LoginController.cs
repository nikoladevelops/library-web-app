using LibraryWebApp.Models;
using LibraryWebApp.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryWebApp.Controllers
{
    public class LoginController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        public LoginController( SignInManager<ApplicationUser> signIn, UserManager<ApplicationUser> um)
        {
            signInManager = signIn;
            userManager = um;
        }

        public async Task<IActionResult> Index()
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
                ApplicationUser user = new ApplicationUser
                {
                    UserName = registerViewModel.Username,
                    Email = registerViewModel.Email
                };
                await userManager.CreateAsync(user, registerViewModel.Password);
                //redirect to book panel here
                return RedirectToAction(nameof(Index));
            }
        return View();
        }
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction(nameof(Index)); ;
        }
    }
}
