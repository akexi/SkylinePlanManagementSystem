using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MockSchoolManagement.Migrations
{
    /// <inheritdoc />
    public partial class addZhangBoNeiStudent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Students",
                columns: new[] { "Id", "Email", "Major", "Name", "PhotoPath" },
                values: new object[] { 3, "zhangsan@163.com", 2, "张波内", "default.jpg" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 3);
        }
    }
}
