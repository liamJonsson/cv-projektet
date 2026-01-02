using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyApp.Migrations
{
    /// <inheritdoc />
    public partial class AddProfileViews : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ProfileViews",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "PasswordHash", "ProfileViews" },
                values: new object[] { "AQAAAAIAAYagAAAAEM8FYG55np8cDoTCPfnLlSPleegc+Mr2mk37wBT8tZZtFEdHgxDUlxryM2Y4vlGPiA==", 0 });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "PasswordHash", "ProfileViews" },
                values: new object[] { "AQAAAAIAAYagAAAAEM2TBTSHr9B7sXLGgqB42FjJ/r80y+JNjlG3N+JTZTJxhw+XViJLLDRN8ez+HZbRfw==", 0 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProfileViews",
                table: "Users");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEHUhRQOfEi/0hTfMuG+BW4ooAQShd1J2N3/0PFUOWxZIKhrFNJOTA41RWBPvZ2Ukag==");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEMN0S17/BkBUBALTo4IvhNTAIX8BldRDHscmFxtzruMFIV+TTNc55W1f5cQ2XjY0xg==");
        }
    }
}
