using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace E_Doctor.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPhysicalExamReportEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PhysicalExamItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Label = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    InputType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhysicalExamItems", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PhysicalExams",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DiagnosisId = table.Column<int>(type: "int", nullable: false),
                    BP = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HR = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RR = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Temp = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Weight = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    O2Sat = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhysicalExams", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PhysicalExamFindings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PhysicalExamId = table.Column<int>(type: "int", nullable: false),
                    PhysicalItemId = table.Column<int>(type: "int", nullable: false),
                    IsNormal = table.Column<bool>(type: "bit", nullable: false),
                    NormalDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AbnormalFindings = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhysicalExamFindings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PhysicalExamFindings_PhysicalExamItems_PhysicalItemId",
                        column: x => x.PhysicalItemId,
                        principalTable: "PhysicalExamItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PhysicalExamFindings_PhysicalExams_PhysicalExamId",
                        column: x => x.PhysicalExamId,
                        principalTable: "PhysicalExams",
                        principalColumn: "Id");
                });

            migrationBuilder.InsertData(
                table: "PhysicalExamItems",
                columns: new[] { "Id", "CreatedOn", "InputType", "IsActive", "Label", "SortOrder", "UpdatedOn" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 11, 17, 15, 14, 25, 948, DateTimeKind.Utc).AddTicks(9593), "", false, "General", 1, null },
                    { 2, new DateTime(2025, 11, 17, 15, 14, 25, 948, DateTimeKind.Utc).AddTicks(9593), "", true, "Head", 2, null },
                    { 3, new DateTime(2025, 11, 17, 15, 14, 25, 948, DateTimeKind.Utc).AddTicks(9593), "", true, "Eyes", 3, null },
                    { 4, new DateTime(2025, 11, 17, 15, 14, 25, 948, DateTimeKind.Utc).AddTicks(9593), "", true, "Ears", 4, null },
                    { 5, new DateTime(2025, 11, 17, 15, 14, 25, 948, DateTimeKind.Utc).AddTicks(9593), "", true, "Nose", 5, null },
                    { 6, new DateTime(2025, 11, 17, 15, 14, 25, 948, DateTimeKind.Utc).AddTicks(9593), "", true, "Throat", 6, null },
                    { 7, new DateTime(2025, 11, 17, 15, 14, 25, 948, DateTimeKind.Utc).AddTicks(9593), "", true, "Neck", 7, null },
                    { 8, new DateTime(2025, 11, 17, 15, 14, 25, 948, DateTimeKind.Utc).AddTicks(9593), "", true, "Breast", 8, null },
                    { 9, new DateTime(2025, 11, 17, 15, 14, 25, 948, DateTimeKind.Utc).AddTicks(9593), "", true, "Chest/Lungs", 9, null },
                    { 10, new DateTime(2025, 11, 17, 15, 14, 25, 948, DateTimeKind.Utc).AddTicks(9593), "", true, "Heart", 10, null },
                    { 11, new DateTime(2025, 11, 17, 15, 14, 25, 948, DateTimeKind.Utc).AddTicks(9593), "", true, "Abdomen", 11, null },
                    { 12, new DateTime(2025, 11, 17, 15, 14, 25, 948, DateTimeKind.Utc).AddTicks(9593), "", true, "Gut", 12, null },
                    { 13, new DateTime(2025, 11, 17, 15, 14, 25, 948, DateTimeKind.Utc).AddTicks(9593), "", true, "Back", 13, null },
                    { 14, new DateTime(2025, 11, 17, 15, 14, 25, 948, DateTimeKind.Utc).AddTicks(9593), "", true, "Extremities", 14, null },
                    { 15, new DateTime(2025, 11, 17, 15, 14, 25, 948, DateTimeKind.Utc).AddTicks(9593), "", true, "Neurologic", 15, null },
                    { 16, new DateTime(2025, 11, 17, 15, 14, 25, 948, DateTimeKind.Utc).AddTicks(9593), "", true, "Skin", 16, null },
                    { 17, new DateTime(2025, 11, 17, 15, 14, 25, 948, DateTimeKind.Utc).AddTicks(9593), "", true, "Others", 17, null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_PhysicalExamFindings_PhysicalExamId",
                table: "PhysicalExamFindings",
                column: "PhysicalExamId");

            migrationBuilder.CreateIndex(
                name: "IX_PhysicalExamFindings_PhysicalItemId",
                table: "PhysicalExamFindings",
                column: "PhysicalItemId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PhysicalExamFindings");

            migrationBuilder.DropTable(
                name: "PhysicalExamItems");

            migrationBuilder.DropTable(
                name: "PhysicalExams");
        }
    }
}
