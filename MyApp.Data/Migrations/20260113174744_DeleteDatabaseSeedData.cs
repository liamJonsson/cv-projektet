using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MyApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class DeleteDatabaseSeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Messages",
                keyColumn: "MessageId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Messages",
                keyColumn: "MessageId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Messages",
                keyColumn: "MessageId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "ProjectUsers",
                keyColumns: new[] { "ProjectId", "UserId" },
                keyValues: new object[] { 1, 1 });

            migrationBuilder.DeleteData(
                table: "ProjectUsers",
                keyColumns: new[] { "ProjectId", "UserId" },
                keyValues: new object[] { 1, 2 });

            migrationBuilder.DeleteData(
                table: "ProjectUsers",
                keyColumns: new[] { "ProjectId", "UserId" },
                keyValues: new object[] { 2, 2 });

            migrationBuilder.DeleteData(
                table: "Projects",
                keyColumn: "ProjectId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Projects",
                keyColumn: "ProjectId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Addresses",
                keyColumn: "AddressId",
                keyValue: 1);

            migrationBuilder.DropColumn(
                name: "Cv",
                table: "Users");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Cv",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.InsertData(
                table: "Addresses",
                columns: new[] { "AddressId", "City", "HomeAddress", "ZipCode" },
                values: new object[] { 1, "Stockholm", "Exempelgatan 10", "123 45" });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "AccessFailedCount", "AddressId", "ConcurrencyStamp", "Cv", "CvImage", "Deactivated", "Education", "Email", "EmailConfirmed", "Experience", "LockoutEnabled", "LockoutEnd", "Name", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "ProfileImage", "ProfileViews", "SecurityStamp", "Skills", "TwoFactorEnabled", "UserName", "Visibility" },
                values: new object[,]
                {
                    { 1, 0, 1, "22222222-2222-2222-2222-222222222222", "cv_lisa.pdf", null, false, null, "lisa.skarf@example.com", true, null, false, null, "Lisa Skarf", "LISA.SKARF@EXAMPLE.COM", "LISASKARF", "AQAAAAIAAYagAAAAEIpSxyP/6fZ291fhEaPXcsGm4m40l761SKxLeBziNO6VEbp7dwlXLEt4qFRAlw2ViQ==", "0720204584", false, "default.jpg", 0, "11111111-1111-1111-1111-111111111111", null, false, "lisaskarf", true },
                    { 2, 0, 1, "44444444-4444-4444-4444-444444444444", "cv_liam.pdf", null, false, null, "liam.jonsson@example.com", true, null, false, null, "Liam Jonsson", "LIAM.JONSSON@EXAMPLE.COM", "LIAMJONSSON", "AQAAAAIAAYagAAAAEEHfZLRjsCJ3ZuZGh6PAtIAdOKdLhUiFhlZmdVNsU233Pk6DXqKtzdT6Nzrt+gvI7w==", "0737528105", false, "default.jpg", 0, "33333333-3333-3333-3333-333333333333", null, false, "liamjonsson", true }
                });

            migrationBuilder.InsertData(
                table: "Messages",
                columns: new[] { "MessageId", "Read", "ReceiverId", "SenderId", "SenderName", "SentAt", "Text" },
                values: new object[,]
                {
                    { 1, false, 2, 1, "Lisa Skarf", new DateTime(2025, 9, 12, 14, 30, 0, 0, DateTimeKind.Unspecified), "Hej hej. Vilket bra projekt. Hur har du tänkt när du gjorde Add-metoden? Vill gärna lära mig av dig. Hör av dig ifall du är intresserad att vara min handledare!!" },
                    { 2, false, 2, 1, "Meja Ammer", new DateTime(2025, 12, 31, 23, 59, 0, 0, DateTimeKind.Unspecified), "Gott nytt år!" },
                    { 3, false, 2, 1, "Lisa Skarf", new DateTime(2025, 12, 29, 8, 0, 0, 0, DateTimeKind.Unspecified), "Hej Liam! Hur har du det på lovet? Har du haft en bra jul? Vi ses snart. Hör gärna av dig. När vi ses ska vi programmera klart systemet. Ha det gott! /Lisa" }
                });

            migrationBuilder.InsertData(
                table: "Projects",
                columns: new[] { "ProjectId", "CodeLanguage", "CreatorId", "Description", "StartDate", "Title" },
                values: new object[,]
                {
                    { 1, "C#", 1, "En enkel konsolapplikation.", new DateTime(2025, 10, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Mitt Första C# Projekt" },
                    { 2, "JavaScript", 2, "En snygg frontend-app.", new DateTime(2025, 9, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), "React Frontend" }
                });

            migrationBuilder.InsertData(
                table: "ProjectUsers",
                columns: new[] { "ProjectId", "UserId" },
                values: new object[,]
                {
                    { 1, 1 },
                    { 1, 2 },
                    { 2, 2 }
                });
        }
    }
}
