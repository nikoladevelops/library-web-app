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
            // Seed several normal users
            List<bool> inserts = new List<bool>
            {
                await SeedSingleUser("AverageUser", "user1@gmail.com", "aaaaaaA!1", AllRoles.User),
                await SeedSingleUser("SecondUser", "user2@gmail.com", "aaaaaaA!1", AllRoles.User),
                await SeedSingleUser("ThirdUser", "user3@gmail.com", "aaaaaaA!1", AllRoles.User),
                await SeedSingleUser("FourthUser", "user4@gmail.com", "aaaaaaA!1", AllRoles.User),
                await SeedSingleUser("FifthUser", "user5@gmail.com", "aaaaaaA!1", AllRoles.User)
            };

            // Abort if any user was not seeded successfully
            foreach (var item in inserts)
            {
                if (!item)
                {
                    return;
                }
            }

            // Seed an admin user
            await SeedSingleUser("admin", "admin@gmail.com", "aaaaaaA!1", AllRoles.Admin);
        }
        /// <summary>
        /// Seeds books into the database, if there are none
        /// </summary>
        public async Task SeedBooks()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                var hasSeededBooksAlready = await context.Books.AnyAsync();

                if (hasSeededBooksAlready)
                {
                    return;
                }
                List<Book> BooksToSeed = new()
                {
                new Book
                {
                    Title = "The Lord of the Rings",
                    PublicationDate = new DateOnly(1954, 7, 29),
                    TotalCount = 10,
                    AvailableCount = 8,
                    Genres = context.Genres.Where(g => g.Name == "Fantasy"|| g.Name == "Adventure").ToList(),
                    Authors = context.Authors.Where(a => a.Name == "J.R.R. Tolkien").ToList(),
                },
                new Book
                {
                    Title = "To Kill a Mockingbird",
                    PublicationDate =  new DateOnly(1960, 7, 11),
                    TotalCount = 7,
                    AvailableCount = 5,
                    Genres = context.Genres.Where(g => g.Name == "Classics"|| g.Name == "Fiction").ToList(),
                    Authors = context.Authors.Where(a => a.Name == "Harper Lee").ToList(),
                },
                new Book
                {
                    Title = "The Metamorphosis",
                    PublicationDate =  new DateOnly(1915, 10, 1),
                    TotalCount = 5,
                    AvailableCount = 3,
                    Genres = context.Genres.Where(g => g.Name == "PhilosophicalFiction"|| g.Name == "Absurdism").ToList(),
                    Authors = context.Authors.Where(a => a.Name == "Franz Kafka").ToList(),
                },
                new Book
                {
                    Title = "One Piece",
                    PublicationDate =  new DateOnly(1997, 7, 22),
                    TotalCount = 20,
                    AvailableCount = 15,
                    Genres = context.Genres.Where(g => g.Name == "Adventure"|| g.Name == "Fantasy").ToList(),
                    Authors = context.Authors.Where(a => a.Name == "Eiichirou Oda").ToList(),
                },
                new Book
                {
                    Title = "The Shining",
                    PublicationDate =  new DateOnly(1977, 1, 28),
                    TotalCount = 12,
                    AvailableCount = 10,
                    Genres = context.Genres.Where(g => g.Name == "Horror"|| g.Name == "ThrillerAndSuspense").ToList(),
                    Authors = context.Authors.Where(a => a.Name == "Stephen King").ToList(),
                }
                };
                foreach (var book in BooksToSeed)
                {
                    if (!context.Books.Any(b => b.Title == book.Title))
                    {
                        context.Books.Add(book);
                    }
                }

                await context.SaveChangesAsync();
            }
        }
        /// <summary>
        /// Seeds Authors into the database, if there are none
        /// </summary>
        public async Task SeedAuthors()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                var hasSeededAuthorsAlready = await context.Authors.AnyAsync();

                if (hasSeededAuthorsAlready)
                {
                    return;
                }
                List<Author> authorsToSeed = new()
                {
                     new() { Name = "J.R.R Tolkien" },
                     new() { Name = "Harper Lee" },
                     new() { Name = "Franz Kafka" },
                     new() { Name = "Eiichirou Oda" },
                     new() { Name = "Stephen King" },
                };
                foreach (var author in authorsToSeed)
                {
                    if (!context.Authors.Any(a => a.Name == author.Name))
                    {
                        context.Authors.Add(author);
                    }
                }

                await context.SaveChangesAsync();
            }
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
                    "Action",
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
                    "Poetry",
                    "PhilosophicalFiction",
                    "Fiction",
                    "Absurdism"
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
        /// Seeds book genres into the database, but only if there are no genres already in the database.
        /// </summary>
        public async Task SeedRentedBooks()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                var hasSeededRentedBooksAlready = await context.RentedBooks.AnyAsync();

                if (hasSeededRentedBooksAlready)
                {
                    return;
                }
                var userIds = context.Users.Select(u => u.Id).Take(5).ToList();
                List<RentedBook> RentedBooksToSeed = new()
                {

                    new RentedBook
                    {
                        BookId = 1, 
                        UserId = userIds[2],
                        RentalDate = new DateOnly(2024, 1, 6),
                        Deadline = new DateOnly(2024, 1, 20),
                        ReturnedAt = null
                    },

                    new RentedBook
                    {
                        BookId = 2, 
                        UserId = userIds[1], 
                        RentalDate = new DateOnly(2024, 1, 10),
                        Deadline = new DateOnly(2024, 1, 24),
                        ReturnedAt = null
                    },

                    new RentedBook
                    {
                        BookId = 3,
                        UserId = userIds[1], 
                        RentalDate = new DateOnly(2024, 1, 1),
                        Deadline = new DateOnly(2024, 1, 15),
                        ReturnedAt = new DateOnly(2024, 1, 14)
                    },

                    new RentedBook
                    {
                        BookId = 4, 
                        UserId = userIds[0], 
                        RentalDate = new DateOnly(2024, 1, 2),
                        Deadline = new DateOnly(2024, 1, 16),
                        ReturnedAt = new DateOnly(2024, 1, 16)
                    },

                    new RentedBook
                    {
                        BookId = 5,
                        UserId = userIds[3],
                        RentalDate = new DateOnly(2024, 1, 3),
                        Deadline = new DateOnly(2024, 1, 17),
                        ReturnedAt = new DateOnly(2024, 1, 20)
                    }
                };
                foreach (var rentedBook in RentedBooksToSeed)
                {
                    if (rentedBook != null)
                    {
                        context.RentedBooks.Add(rentedBook);
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
