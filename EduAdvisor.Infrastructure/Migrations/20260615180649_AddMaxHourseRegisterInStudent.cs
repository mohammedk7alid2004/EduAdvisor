using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EduAdvisor.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddMaxHourseRegisterInStudent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CustomMaxCreditHours",
                table: "Students",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CustomMaxCreditHours",
                table: "Students");
        }
    }
}
