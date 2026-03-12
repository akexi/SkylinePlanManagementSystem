using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MockSchoolManagement.Migrations
{
    /// <inheritdoc />
    public partial class SeedStudentsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Students",
                columns: new[] { "Id", "Email", "Major", "Name" },
                values: new object[] { 1, "xsbnltx@gmail.com", 1, "李天喜" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 1);
        }
    }
}
