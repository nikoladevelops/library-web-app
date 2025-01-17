using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LibraryWebApp.Migrations
{
    /// <inheritdoc />
    public partial class RenameReturnDateToDeadlineToAvoidConfusion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ReturnDate",
                table: "RentedBooks",
                newName: "Deadline");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Deadline",
                table: "RentedBooks",
                newName: "ReturnDate");
        }
    }
}
