using LibraryWebApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Data;
using static LibraryWebApp.Globals;

namespace LibraryWebApp.Helpers
{
    /// <summary>
    /// Seeds data
    /// </summary>
    public class DataSeeder
    {
        private readonly IServiceProvider _serviceProvider;
        public DataSeeder(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        /// <summary>
        /// Seeds roles into the database. If they already exist, it does nothing.
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <returns></returns>
        public async Task SeedRoles() 
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

                // If even the first role exists, we assume that all roles exist and abort
                if (await roleManager.RoleExistsAsync(Roles.User))
                {
                    return;
                }

                var roles = Enum.GetNames(typeof(AllRoles));

                foreach (var role in roles)
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }
        }

        /// <summary>
        /// Seeds users into the database. If they already exist, it does nothing
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <returns></returns>
        public async Task SeedUsers()
        {
            // Seed a normal user
            bool success = await SeedSingleUser("user", "user@gmail.com", "uuuuuuU!1", AllRoles.User);
            
            // Abort if the user was not seeded successfully
            if (success == false)
            {
                return;
            }

            // Seed an admin user
            await SeedSingleUser("admin", "admin@gmail.com", "aaaaaaA!1", AllRoles.Admin);
        }

        /// <summary>
        /// Seeds book genres into the database, but only if there are no genres already in the database.
        /// </summary>
        public async Task SeedBookGenres() 
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                string[] genresToSeed =
                {
                    "ActionAndAdventure",
                    "Classics",
                    "ComicAndGraphicNovel",
                    "CrimeAndMystery",
                    "Drama",
                    "Fantasy",
                    "HistoricalFiction",
                    "Horror",
                    "LiteraryFiction",
                    "NonFiction",
                    "Romance",
                    "ScienceFiction",
                    "ThrillerAndSuspense",
                    "BiographyAndMemoir",
                    "Poetry"
                };

                var hasSeededGenresAlready = await context.Genres.AnyAsync();

                if (hasSeededGenresAlready)
                {
                    return;
                }

                foreach (var genreName in genresToSeed)
                {
                    if (!context.Genres.Any(g => g.Name == genreName))
                    {
                        context.Genres.Add(new Genre { Name = genreName });
                    }
                }

                await context.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Seeds a single user into the database.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <param name="role"></param>
        /// <param name="serviceProvider"></param>
        /// <returns>Whether the user was seeded successfully</returns>
        private async Task<bool> SeedSingleUser(string username, string email, string password, AllRoles role) 
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

                // If user with this email already exists, abort
                if (await userManager.FindByEmailAsync(email) != null)
                {
                    return false;
                }

                // If user with this username already exists, abort
                if (await userManager.FindByNameAsync(username) != null)
                {
                    return false;
                }

                var user = new ApplicationUser
                {
                    IsBanned = false,
                    UserName = username,
                    Email = email
                };

                var createUserResult = await userManager.CreateAsync(user, password);

                if (createUserResult.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, role.ToString());
                }
            }

            return true;
        }
    }
}
