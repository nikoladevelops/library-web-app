using LibraryWebApp.Models;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Read the .env file in the project directory, automatically adds all those key value pairs as environment variables that can be accessed in runtime (you should create that file and have that DotNetEnv package as I mentioned in the README.md)
DotNetEnv.Env.Load();

string? connection_string = Environment.GetEnvironmentVariable("CONNECTION_STRING");
if (connection_string == null) 
{
    throw new Exception("You haven't configured the connection string, check Program.cs and the .env file");
}

// Sets up the db context, so now we can communicate with the database
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connection_string));


// Add Identity support + tables
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();


// Build the app
var app = builder.Build();


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Makes it possible to validate requests / who the user is /  checking for a valid token or cookie (the client's details are saved into User.Identity.Name, User.Claims)
app.UseAuthentication();

// Based on the previous Authentication user details, checks if the user has the valid Roles / Claims / Policies (basically allows attributes like these to work: [Authorize(Roles = "Admin")] )
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
