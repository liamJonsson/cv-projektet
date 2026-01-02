using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyApp.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSeedDataMessage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ZipFile",
                table: "Projects",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<DateTime>(
                name: "StartDate",
                table: "Projects",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateOnly),
                oldType: "date",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "EndDate",
                table: "Projects",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateOnly),
                oldType: "date",
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "Messages",
                keyColumn: "MessageId",
                keyValue: 1,
                columns: new[] { "SentAt", "Text" },
                values: new object[] { new DateTime(2025, 9, 12, 14, 30, 0, 0, DateTimeKind.Unspecified), "Hej hej. Vilket bra projekt. Hur har du tänkt när du gjorde Add-metoden? Vill gärna lära mig av dig. Hör av dig ifall du är intresserad att vara min handledare!!" });

            migrationBuilder.UpdateData(
                table: "Messages",
                keyColumn: "MessageId",
                keyValue: 2,
                columns: new[] { "SenderName", "SentAt", "Text" },
                values: new object[] { "Meja Ammer", new DateTime(2025, 12, 31, 23, 59, 0, 0, DateTimeKind.Unspecified), "Gott nytt år!" });

            migrationBuilder.UpdateData(
                table: "Messages",
                keyColumn: "MessageId",
                keyValue: 3,
                columns: new[] { "SenderName", "SentAt", "Text" },
                values: new object[] { "Lisa Skarf", new DateTime(2025, 12, 29, 8, 0, 0, 0, DateTimeKind.Unspecified), "Hej Liam! Hur har du det på lovet? Har du haft en bra jul? Vi ses snart. Hör gärna av dig. När vi ses ska vi programmera klart systemet. Ha det gott! /Lisa" });

            migrationBuilder.UpdateData(
                table: "Projects",
                keyColumn: "ProjectId",
                keyValue: 1,
                columns: new[] { "EndDate", "StartDate" },
                values: new object[] { new DateTime(2025, 12, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 10, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "Projects",
                keyColumn: "ProjectId",
                keyValue: 2,
                columns: new[] { "EndDate", "StartDate" },
                values: new object[] { new DateTime(2025, 12, 23, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 9, 12, 0, 0, 0, 0, DateTimeKind.Unspecified) });

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ZipFile",
                table: "Projects",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateOnly>(
                name: "StartDate",
                table: "Projects",
                type: "date",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateOnly>(
                name: "EndDate",
                table: "Projects",
                type: "date",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "Messages",
                keyColumn: "MessageId",
                keyValue: 1,
                columns: new[] { "SentAt", "Text" },
                values: new object[] { new DateTime(2025, 12, 29, 14, 59, 55, 208, DateTimeKind.Local).AddTicks(8818), "Hej" });

            migrationBuilder.UpdateData(
                table: "Messages",
                keyColumn: "MessageId",
                keyValue: 2,
                columns: new[] { "SenderName", "SentAt", "Text" },
                values: new object[] { "Lisa S", new DateTime(2025, 12, 29, 14, 59, 55, 212, DateTimeKind.Local).AddTicks(995), "Hej igen" });

            migrationBuilder.UpdateData(
                table: "Messages",
                keyColumn: "MessageId",
                keyValue: 3,
                columns: new[] { "SenderName", "SentAt", "Text" },
                values: new object[] { "Lisa S", new DateTime(2025, 12, 29, 14, 59, 55, 212, DateTimeKind.Local).AddTicks(1017), "Hej på dig hahahbdhbdhabdhabdhb hdbhadb hdbhbdahbd hdbabhbdhahdba hbdabdhbah hbadhdabhdb hbaddhbb. habdha hadbhbd ahdbadhb hadbhad habdhadb ahdbhd badhbd badhd habdhadbhabd hbdahd hbadbdah hbadhbd badhdb bdahdb bahdbd abdhadb badibdadb Hejdå!" });

            migrationBuilder.UpdateData(
                table: "Projects",
                keyColumn: "ProjectId",
                keyValue: 1,
                columns: new[] { "EndDate", "StartDate" },
                values: new object[] { new DateOnly(2025, 12, 15), new DateOnly(2025, 10, 1) });

            migrationBuilder.UpdateData(
                table: "Projects",
                keyColumn: "ProjectId",
                keyValue: 2,
                columns: new[] { "EndDate", "StartDate" },
                values: new object[] { new DateOnly(2025, 12, 23), new DateOnly(2025, 9, 12) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAED65qFTCjhjI9Xu31Y4ZwRA+9iNvgOYYqN1bNP+Vwp8dnhOuOzDbLS8PaU3u8uJguw==");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEAczylhsXLL2kvRLaVzRSc+cfZw5fX0V18sJwxhCzB2XJ+zR4SgmcrbOdDUNql7baQ==");
        }
    }
}
