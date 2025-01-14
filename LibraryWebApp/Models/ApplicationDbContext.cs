using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace LibraryWebApp.Models
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options):base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //DO NOT REMOVE THIS FROM ONCONFIGURING, YOUR MIGRATIONS WILL BE FUCKED
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.ConfigureWarnings(warnings => warnings.Ignore(RelationalEventId.PendingModelChangesWarning));
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<IdentityRole>().HasData(
          new IdentityRole { Id = "1", Name = "Admin", NormalizedName = "ADMIN" },
          new IdentityRole { Id = "2", Name = "User", NormalizedName = "USER" } 
      );
            var adminUser = new ApplicationUser
            {
                Id = "11",
                UserName = "adminSeed",
                Email = "adminSeed@ex.com",
                NormalizedUserName = "ADMINSEED",
                NormalizedEmail = "ADMINSEED@EX.BG",
                PasswordHash = new PasswordHasher<ApplicationUser>().HashPassword(null, "Admin@123!")
            };

            var normalUser = new ApplicationUser
            {
                Id = "22", 
                UserName = "userSeed",
                Email = "userSeed@ex.bg",
                NormalizedUserName = "USERSEED",
                NormalizedEmail = "USERSEED@EX.BG",
                PasswordHash = new PasswordHasher<ApplicationUser>().HashPassword(null, "User@123!")
            };
            builder.Entity<ApplicationUser>().HasData(adminUser, normalUser);

            builder.Entity<IdentityUserRole<string>>().HasData(
                new IdentityUserRole<string>
                {
                    UserId = "11", 
                    RoleId = "1"     
                },
                new IdentityUserRole<string>
                {
                    UserId = "22", 
                    RoleId = "2"   
                }
            );
        }
        public DbSet<Book> Books { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<Author> Authors { get; set; }
    }
}
