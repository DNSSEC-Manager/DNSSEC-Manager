using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend.Data.Migrations
{
    public partial class AddDefaultUserAccount : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "UserName", "NormalizedUserName", "Email", "NormalizedEmail", "EmailConfirmed", "PasswordHash", "SecurityStamp", "ConcurrencyStamp", "PhoneNumberConfirmed", "TwoFactorEnabled", "LockoutEnabled", "AccessFailedCount" },
                values: new object[] { "04988785-1430-4fa0-8bf6-e5b469b21d13", "admin", "ADMIN", "contact@dnssec-script.com", "CONTACT@DNSSEC-SCRIPT.COM", true, "AQAAAAEAACcQAAAAEO1zIIVvUGH50dSr76trmq7LhhD3jyY0UXzM6f/8G3Y8KgRQ2kBK+BwVp0OukSnJTA==", "4F4BIAP2BYB3FBBE52XHPECPPWKPZA3G", "834a62ab-4948-4a22-a068-b249ebed1201", false, false, false, 0 });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "04988785-1430-4fa0-8bf6-e5b469b21d13");
        }
    }
}
