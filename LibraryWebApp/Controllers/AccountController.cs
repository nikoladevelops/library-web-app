﻿using LibraryWebApp.Helpers;
using LibraryWebApp.Models;
using LibraryWebApp.ViewModels;
using Microsoft.AspNetCore.Authorization;
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

        public AccountController(SignInManager<ApplicationUser> signIn, UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _signInManager = signIn;
            _userManager = userManager;
            _context = context;
        }

        public IActionResult Register()
        {
            return View();
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterVM registerViewModel)
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
                    IsBanned = false,
                    UserName = registerViewModel.Username,
                    Email = registerViewModel.Email,
                    PhoneNumber = registerViewModel.PhoneNumber
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginVM vm)
        {
            if (ModelState.IsValid)
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == vm.Username);
                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return View(vm);

                }
            
                var res = await _signInManager.PasswordSignInAsync(vm.Username, vm.Password, false, false);

                if (res.Succeeded)
                {
                    if (user.IsBanned)
                    {
                        return View("Error",ErrorViewModelTypes.UserBanned());
                    }

                    return RedirectToAction("Index", "Home");
                }
                else 
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                }
            }

            return View(vm);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
