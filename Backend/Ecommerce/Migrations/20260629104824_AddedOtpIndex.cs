using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Ecommerce.Migrations
{
    /// <inheritdoc />
    public partial class AddedOtpIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "a234774c-02f1-4afd-93ce-3526be132a08");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "db311b53-f11a-49e4-97ab-213402c0f6bf");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "5fdf2988-afaa-4279-a896-8ede72d7153d", null, "User", "USER" },
                    { "dfa8282b-d3f1-4674-b122-d0a58cea1638", null, "Admin", "ADMIN" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_OtpVerifications_Email_CreatedAt",
                table: "OtpVerifications",
                columns: new[] { "Email", "CreatedAt" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_OtpVerifications_Email_CreatedAt",
                table: "OtpVerifications");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "5fdf2988-afaa-4279-a896-8ede72d7153d");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "dfa8282b-d3f1-4674-b122-d0a58cea1638");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "a234774c-02f1-4afd-93ce-3526be132a08", null, "User", "USER" },
                    { "db311b53-f11a-49e4-97ab-213402c0f6bf", null, "Admin", "ADMIN" }
                });
        }
    }
}
