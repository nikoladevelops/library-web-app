## Library Web App

Currently In development..


## How to run project locally - instructions:

1. Update [Visual Studio 2022](https://visualstudio.microsoft.com/vs/) to latest
2. Download [.NET 9](https://dotnet.microsoft.com/en-us/download/dotnet/9.0)
3. Clone the repository:
```
git clone https://github.com/nikoladevelops/library-web-app.git
```

5. Open `LibraryWebApp.sln` in Visual Studio 2022
6. Open `Package Manager Console`

(Tools -> NuGet Package Manager -> Package Manager Console)

8. Run this command inside the `Package Manager Console`:
```
dotnet restore
```
(this will just download all NuGet packages locally, all those packages are mandatory since the project won't work without them)

10. Create a brand new `.env` file inside the project - this is where all secrets/environment variables will be at (this file should never be uploaded to your remote, that's why the `.gitignore` already ignores it)

9. Open this brand new empty `.env` file that you just created and configure it with your own SQL Server connection string:

```
CONNECTION_STRING=connection_string_goes_here
```
(replace `connection_string_goes_here` with your actual connection string)


Connection string for development example:

```
CONNECTION_STRING=Server=.\SQLEXPRESS;Database=LibraryDB;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True;
```


11. If all packages have been installed and your connection string is correct (and also all necessary SQL Server services are running on your device) all that is left to do is to apply all migrations so that your database can be generated.
This is done by running the following command inside the `Package Manager Console`:

```
update-database
```

13. Run the project and test it


## How to change database tables / add new tables / change table attributes?

### 1. Add new Model classes or just edit existing ones
### 2. Run migration commands inside the Package Manager Console

Migration Commands cheat sheet:

```
add-migration migration_name_goes_here
```
(this generates a migration file with changes that should be applied to the actual SQL Server Database)

After running the first command the database changes are not yet applied.
In order to apply the changes that the generated migration file describes, you need to run this command:

```
update-database
```

In case you've made a mistake by generating a migration that you don't really need and you don't
want to apply to the actual database (you still haven't run the update-database command), you can remove it with this command:

```
remove-migration
```

If you've done something <b>truly terrible beyond human imagination</b>, you can always drop the database from SQL Server SSMS, delete bad migrations from the folder or edit/tweak them and finnaly run update-database so all tables can be generated again - only problem is loss of all data that has already been saved to the tables

Any other changes related to the project code do not depend on migration commands, so you are safe to do whatever.

