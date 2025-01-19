using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LibraryWebApp.Models;
using LibraryWebApp.ViewModels;
using Microsoft.AspNetCore.Authorization;
using LibraryWebApp.Helpers;

namespace LibraryWebApp.Controllers
{
    public class AuthorController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AuthorController(ApplicationDbContext context)
        {
            _context = context;
        }

        [Authorize(Roles = Globals.Roles.Admin)]
        public async Task<IActionResult> Index()
        {
            return View(await _context.Authors.ToListAsync());
        }

        [Authorize(Roles = Globals.Roles.Admin)]
        public async Task<IActionResult> Details(int id)
        {
            var author = await _context.Authors.FindAsync(id);
            if (author == null)
            {
                return View("Error", ErrorViewModelTypes.NotFound("author"));
            }

            List<Book>? list = _context.Books.Where(b => b.Authors.Contains(author)).ToList();

            AuthorDetailsVM model = new AuthorDetailsVM
            {
                Author = author,
                Books = list
            };

            return View(model);
        }

        [Authorize(Roles = Globals.Roles.Admin)]
        public IActionResult Create()
        {
            return View();
        }

        [Authorize(Roles = Globals.Roles.Admin)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Author author)
        {
            if (ModelState.IsValid)
            {
                _context.Add(author);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(author);
        }


        [Authorize(Roles = Globals.Roles.Admin)]
        public async Task<IActionResult> Edit(int id)
        {
            var author = await _context.Authors.FindAsync(id);
            if (author == null)
            {
                return View("Error", ErrorViewModelTypes.NotFound("author"));
            }
            return View(author);
        }

        [Authorize(Roles = Globals.Roles.Admin)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Author author)
        {
            if (_context.Authors.Any(a => a.Id == author.Id) == false) 
            {
                return View("Error", ErrorViewModelTypes.NotFound("author"));
            }

            if (ModelState.IsValid)
            {
                _context.Update(author);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            return View(author);
        }


        [Authorize(Roles = Globals.Roles.Admin)]
        public async Task<IActionResult> Delete(int id)
        {
            var author = await _context.Authors.FindAsync(id);
            if (author == null)
            {
                return View("Error", ErrorViewModelTypes.NotFound("author"));
            }

            return View(author);
        }

        [Authorize(Roles = Globals.Roles.Admin)]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePOST(int id)
        {
            var author = await _context.Authors.FindAsync(id);
            if (author != null)
            {
                _context.Authors.Remove(author);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
