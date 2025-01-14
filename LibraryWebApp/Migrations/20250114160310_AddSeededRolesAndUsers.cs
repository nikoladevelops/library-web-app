using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace LibraryWebApp.Migrations
{
    /// <inheritdoc />
    public partial class AddSeededRolesAndUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "1", null, "Admin", "ADMIN" },
                    { "2", null, "User", "USER" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[,]
                {
                    { "11", 0, "effbbb34-17fc-4aa0-934e-ecea203cb08a", "adminSeed@ex.com", false, false, null, "ADMINSEED@EX.BG", "ADMINSEED", "AQAAAAIAAYagAAAAEF2PRDX6Lo2OR0eYoq1G2TDmt0E7vk/eNtBkhOEHovkb080+kdIeN4mbpM4zCOAWCQ==", null, false, "ce360bee-8c4f-4571-bc9c-a66fbef7fb6c", false, "adminSeed" },
                    { "22", 0, "be739322-efd9-4c51-9524-a805c2fdcc5c", "userSeed@ex.bg", false, false, null, "USERSEED@EX.BG", "USERSEED", "AQAAAAIAAYagAAAAEDksjNhk3Oe5ZOG8Ua8P3QM3xt/O7BPszNyr8I56y9VOW9d/+c6E2dFK/dpCqXiyDA==", null, false, "b14d59c6-e39d-4e20-be3b-7c2650d1e8c4", false, "userSeed" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[,]
                {
                    { "1", "11" },
                    { "2", "22" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "1", "11" });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "2", "22" });

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "2");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "11");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "22");
        }
    }
}
