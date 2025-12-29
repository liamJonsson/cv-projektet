using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyApp.Migrations
{
    /// <inheritdoc />
    public partial class AddSentAtAndSeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "SentAt",
                table: "Messages",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "Messages",
                keyColumn: "MessageId",
                keyValue: 1,
                column: "SentAt",
                value: new DateTime(2025, 12, 29, 14, 59, 55, 208, DateTimeKind.Local).AddTicks(8818));

            migrationBuilder.UpdateData(
                table: "Messages",
                keyColumn: "MessageId",
                keyValue: 2,
                column: "SentAt",
                value: new DateTime(2025, 12, 29, 14, 59, 55, 212, DateTimeKind.Local).AddTicks(995));

            migrationBuilder.InsertData(
                table: "Messages",
                columns: new[] { "MessageId", "Read", "ReceiverId", "SenderId", "SenderName", "SentAt", "Text" },
                values: new object[] { 3, false, 2, 1, "Lisa S", new DateTime(2025, 12, 29, 14, 59, 55, 212, DateTimeKind.Local).AddTicks(1017), "Hej på dig hahahbdhbdhabdhabdhb hdbhadb hdbhbdahbd hdbabhbdhahdba hbdabdhbah hbadhdabhdb hbaddhbb. habdha hadbhbd ahdbadhb hadbhad habdhadb ahdbhd badhbd badhd habdhadbhabd hbdahd hbadbdah hbadhbd badhdb bdahdb bahdbd abdhadb badibdadb Hejdå!" });

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Messages",
                keyColumn: "MessageId",
                keyValue: 3);

            migrationBuilder.DropColumn(
                name: "SentAt",
                table: "Messages");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEDU1eGuHQ2QrKxaCMtnxqubvWlee1tkZi/vE9hchIQGzTOLwXXauKt3DMRl3RRfksw==");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAENS5+PEmRg3CT9Y8k3YFOz6kEY2ndr4TjTBDTeNH4ybCrJUPKC/mdqp23j+PAIkjfA==");
        }
    }
}
