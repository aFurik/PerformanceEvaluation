using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PerformanceEvaluation.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Competencies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Competencies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Employees",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FullName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Position = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Department = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Role = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employees", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EvaluationSessions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EvaluationSessions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AnonymousMappings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SessionId = table.Column<int>(type: "int", nullable: false),
                    EvaluatorEmployeeId = table.Column<int>(type: "int", nullable: false),
                    AnonymousCode = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnonymousMappings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AnonymousMappings_Employees_EvaluatorEmployeeId",
                        column: x => x.EvaluatorEmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AnonymousMappings_EvaluationSessions_SessionId",
                        column: x => x.SessionId,
                        principalTable: "EvaluationSessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EvaluationResults",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SessionId = table.Column<int>(type: "int", nullable: false),
                    EvaluatedEmployeeId = table.Column<int>(type: "int", nullable: false),
                    EvaluatorEmployeeId = table.Column<int>(type: "int", nullable: false),
                    CompetencyId = table.Column<int>(type: "int", nullable: false),
                    Score = table.Column<int>(type: "int", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EvaluationResults", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EvaluationResults_Competencies_CompetencyId",
                        column: x => x.CompetencyId,
                        principalTable: "Competencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EvaluationResults_Employees_EvaluatedEmployeeId",
                        column: x => x.EvaluatedEmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EvaluationResults_Employees_EvaluatorEmployeeId",
                        column: x => x.EvaluatorEmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EvaluationResults_EvaluationSessions_SessionId",
                        column: x => x.SessionId,
                        principalTable: "EvaluationSessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AnonymousMappings_Code",
                table: "AnonymousMappings",
                column: "AnonymousCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AnonymousMappings_EvaluatorEmployeeId",
                table: "AnonymousMappings",
                column: "EvaluatorEmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_AnonymousMappings_Session_Evaluator",
                table: "AnonymousMappings",
                columns: new[] { "SessionId", "EvaluatorEmployeeId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Employees_Department",
                table: "Employees",
                column: "Department");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_Email",
                table: "Employees",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Employees_Role",
                table: "Employees",
                column: "Role");

            migrationBuilder.CreateIndex(
                name: "IX_EvaluationResults_CompetencyId",
                table: "EvaluationResults",
                column: "CompetencyId");

            migrationBuilder.CreateIndex(
                name: "IX_EvaluationResults_EvaluatedEmployeeId",
                table: "EvaluationResults",
                column: "EvaluatedEmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_EvaluationResults_EvaluatorEmployeeId",
                table: "EvaluationResults",
                column: "EvaluatorEmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_EvaluationResults_Session_Evaluated",
                table: "EvaluationResults",
                columns: new[] { "SessionId", "EvaluatedEmployeeId" });

            migrationBuilder.CreateIndex(
                name: "IX_EvaluationResults_Session_Evaluator",
                table: "EvaluationResults",
                columns: new[] { "SessionId", "EvaluatorEmployeeId" });

            migrationBuilder.CreateIndex(
                name: "IX_EvaluationResults_Unique_Evaluation",
                table: "EvaluationResults",
                columns: new[] { "SessionId", "EvaluatorEmployeeId", "EvaluatedEmployeeId", "CompetencyId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EvaluationSessions_DateRange",
                table: "EvaluationSessions",
                columns: new[] { "StartDate", "EndDate" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AnonymousMappings");

            migrationBuilder.DropTable(
                name: "EvaluationResults");

            migrationBuilder.DropTable(
                name: "Competencies");

            migrationBuilder.DropTable(
                name: "Employees");

            migrationBuilder.DropTable(
                name: "EvaluationSessions");
        }
    }
}
