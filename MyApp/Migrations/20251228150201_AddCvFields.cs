using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyApp.Migrations
{
    /// <inheritdoc />
    public partial class AddCvFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Education",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Experience",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Skills",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Education", "Experience", "PasswordHash", "Skills" },
                values: new object[] { null, null, "AQAAAAIAAYagAAAAENdccBMZDegV12sbPinyvj15w1LHz8AN6VTeTWvhV/zEXBjsuFLBPh/lOsQfbzx0PQ==", null });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Education", "Experience", "PasswordHash", "Skills" },
                values: new object[] { null, null, "AQAAAAIAAYagAAAAEC1Qn9Vzcb8OhPLX15C/UdmP3cabPTMjDqfe4k5LZzADpb5WbDzhYZ4dv9v1ckPXeA==", null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Education",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Experience",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Skills",
                table: "Users");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEMVvvJ3naeV9oQy0oJtDdBFTRJgNn0NEd6xKaboBvxHfqhTPrunfTkxCRQmXoCZcsA==");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEOG3pzGq4Z7+Me0J51pDykvPQB3P7hloRBb/ufqmiMocgYrvjBRQwKRMuuuIxWfLvw==");
        }
    }
}
