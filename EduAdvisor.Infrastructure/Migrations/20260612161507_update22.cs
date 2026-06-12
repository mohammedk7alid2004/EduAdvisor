using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EduAdvisor.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class update22 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Enrollments_Courses_CourseId",
                table: "Enrollments");

            migrationBuilder.DropForeignKey(
                name: "FK_Enrollments_Semesters_SemesterId",
                table: "Enrollments");

            migrationBuilder.DropColumn(
                name: "Grade",
                table: "Enrollments");

            migrationBuilder.RenameColumn(
                name: "SemesterId",
                table: "Enrollments",
                newName: "SemesterCourseId");

            migrationBuilder.RenameColumn(
                name: "CourseId",
                table: "Enrollments",
                newName: "RegistrationRequestId");

            migrationBuilder.RenameIndex(
                name: "IX_Enrollments_SemesterId",
                table: "Enrollments",
                newName: "IX_Enrollments_SemesterCourseId");

            migrationBuilder.RenameIndex(
                name: "IX_Enrollments_CourseId",
                table: "Enrollments",
                newName: "IX_Enrollments_RegistrationRequestId");

            migrationBuilder.AddColumn<int>(
                name: "StandardSemesterNumber",
                table: "Semesters",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "CourseGpa",
                table: "Enrollments",
                type: "decimal(3,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "CoursePercentage",
                table: "Enrollments",
                type: "decimal(5,2)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CourseAcademicPlans",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CourseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Level = table.Column<int>(type: "int", nullable: false),
                    StandardSemester = table.Column<int>(type: "int", nullable: false),
                    DepartmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedById = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedById = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedById = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseAcademicPlans", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CourseAcademicPlans_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CourseAcademicPlans_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CourseAcademicPlans_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CourseAcademicPlans_Departments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Departments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "RegistrationRequests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StudentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SemesterId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    SubmittedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReviewedByAdvisorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedById = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedById = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedById = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegistrationRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RegistrationRequests_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_RegistrationRequests_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_RegistrationRequests_Semesters_SemesterId",
                        column: x => x.SemesterId,
                        principalTable: "Semesters",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_RegistrationRequests_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "SemesterCourses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SemesterId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CourseAcademicPlanId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedById = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedById = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedById = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SemesterCourses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SemesterCourses_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SemesterCourses_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SemesterCourses_CourseAcademicPlans_CourseAcademicPlanId",
                        column: x => x.CourseAcademicPlanId,
                        principalTable: "CourseAcademicPlans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SemesterCourses_Semesters_SemesterId",
                        column: x => x.SemesterId,
                        principalTable: "Semesters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CourseAcademicPlans_CourseId",
                table: "CourseAcademicPlans",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseAcademicPlans_CreatedById",
                table: "CourseAcademicPlans",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_CourseAcademicPlans_DepartmentId",
                table: "CourseAcademicPlans",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseAcademicPlans_UpdatedById",
                table: "CourseAcademicPlans",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_RegistrationRequests_CreatedById",
                table: "RegistrationRequests",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_RegistrationRequests_SemesterId",
                table: "RegistrationRequests",
                column: "SemesterId");

            migrationBuilder.CreateIndex(
                name: "IX_RegistrationRequests_StudentId",
                table: "RegistrationRequests",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_RegistrationRequests_UpdatedById",
                table: "RegistrationRequests",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_SemesterCourses_CourseAcademicPlanId",
                table: "SemesterCourses",
                column: "CourseAcademicPlanId");

            migrationBuilder.CreateIndex(
                name: "IX_SemesterCourses_CreatedById",
                table: "SemesterCourses",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_SemesterCourses_SemesterId",
                table: "SemesterCourses",
                column: "SemesterId");

            migrationBuilder.CreateIndex(
                name: "IX_SemesterCourses_UpdatedById",
                table: "SemesterCourses",
                column: "UpdatedById");

            migrationBuilder.AddForeignKey(
                name: "FK_Enrollments_RegistrationRequests_RegistrationRequestId",
                table: "Enrollments",
                column: "RegistrationRequestId",
                principalTable: "RegistrationRequests",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Enrollments_SemesterCourses_SemesterCourseId",
                table: "Enrollments",
                column: "SemesterCourseId",
                principalTable: "SemesterCourses",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Enrollments_RegistrationRequests_RegistrationRequestId",
                table: "Enrollments");

            migrationBuilder.DropForeignKey(
                name: "FK_Enrollments_SemesterCourses_SemesterCourseId",
                table: "Enrollments");

            migrationBuilder.DropTable(
                name: "RegistrationRequests");

            migrationBuilder.DropTable(
                name: "SemesterCourses");

            migrationBuilder.DropTable(
                name: "CourseAcademicPlans");

            migrationBuilder.DropColumn(
                name: "StandardSemesterNumber",
                table: "Semesters");

            migrationBuilder.DropColumn(
                name: "CourseGpa",
                table: "Enrollments");

            migrationBuilder.DropColumn(
                name: "CoursePercentage",
                table: "Enrollments");

            migrationBuilder.RenameColumn(
                name: "SemesterCourseId",
                table: "Enrollments",
                newName: "SemesterId");

            migrationBuilder.RenameColumn(
                name: "RegistrationRequestId",
                table: "Enrollments",
                newName: "CourseId");

            migrationBuilder.RenameIndex(
                name: "IX_Enrollments_SemesterCourseId",
                table: "Enrollments",
                newName: "IX_Enrollments_SemesterId");

            migrationBuilder.RenameIndex(
                name: "IX_Enrollments_RegistrationRequestId",
                table: "Enrollments",
                newName: "IX_Enrollments_CourseId");

            migrationBuilder.AddColumn<decimal>(
                name: "Grade",
                table: "Enrollments",
                type: "decimal(4,2)",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Enrollments_Courses_CourseId",
                table: "Enrollments",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Enrollments_Semesters_SemesterId",
                table: "Enrollments",
                column: "SemesterId",
                principalTable: "Semesters",
                principalColumn: "Id");
        }
    }
}
