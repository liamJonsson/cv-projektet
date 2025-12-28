using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyApp.Migrations
{
    /// <inheritdoc />
    public partial class AddCvImageField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ProfileImage",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "CvImage",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CvImage", "PasswordHash" },
                values: new object[] { null, "AQAAAAIAAYagAAAAECBxtHJYTe+GlzaRQMGBHiVQGCCIgHdtyDe57YBHAhkYTKd6zBaykGnDEHru4QSSTw==" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CvImage", "PasswordHash" },
                values: new object[] { null, "AQAAAAIAAYagAAAAEBDZ7L/Kw6oN4fTqcqotoH5xIIkBq3Z4JQgrY2GoBb7G/ptjy1RNdQZHSE4MEzx9RQ==" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CvImage",
                table: "Users");

            migrationBuilder.AlterColumn<string>(
                name: "ProfileImage",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAENdccBMZDegV12sbPinyvj15w1LHz8AN6VTeTWvhV/zEXBjsuFLBPh/lOsQfbzx0PQ==");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEC1Qn9Vzcb8OhPLX15C/UdmP3cabPTMjDqfe4k5LZzADpb5WbDzhYZ4dv9v1ckPXeA==");
        }
    }
}
