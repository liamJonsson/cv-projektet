using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyApp.Migrations
{
    /// <inheritdoc />
    public partial class AddNewSeedMessage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Messages",
                columns: new[] { "MessageId", "Read", "ReceiverId", "SenderId", "SenderName", "Text" },
                values: new object[] { 2, false, 2, 1, "Lisa S", "Hej igen" });

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Messages",
                keyColumn: "MessageId",
                keyValue: 2);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAECBxtHJYTe+GlzaRQMGBHiVQGCCIgHdtyDe57YBHAhkYTKd6zBaykGnDEHru4QSSTw==");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEBDZ7L/Kw6oN4fTqcqotoH5xIIkBq3Z4JQgrY2GoBb7G/ptjy1RNdQZHSE4MEzx9RQ==");
        }
    }
}
