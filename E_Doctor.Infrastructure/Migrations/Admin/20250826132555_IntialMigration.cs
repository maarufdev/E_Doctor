using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace E_Doctor.Infrastructure.Migrations.Admin
{
    /// <inheritdoc />
    public partial class IntialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Illnesses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    IllnessName = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "TEXT", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Illnesses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Symptoms",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "TEXT", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Symptoms", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "IllnessRules",
                columns: table => new
                {
                    SymptomId = table.Column<int>(type: "INTEGER", nullable: false),
                    IllnessId = table.Column<int>(type: "INTEGER", nullable: false),
                    Condition = table.Column<int>(type: "INTEGER", nullable: false),
                    Days = table.Column<int>(type: "INTEGER", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IllnessRules", x => new { x.IllnessId, x.SymptomId });
                    table.ForeignKey(
                        name: "FK_IllnessRules_Illnesses_IllnessId",
                        column: x => x.IllnessId,
                        principalTable: "Illnesses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_IllnessRules_Symptoms_SymptomId",
                        column: x => x.SymptomId,
                        principalTable: "Symptoms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Illnesses",
                columns: new[] { "Id", "CreatedOn", "Description", "IllnessName", "IsActive", "UpdatedOn" },
                values: new object[] { 1, new DateTime(2025, 8, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), "Common", "Flu", true, null });

            migrationBuilder.InsertData(
                table: "Symptoms",
                columns: new[] { "Id", "CreatedOn", "IsActive", "Name", "UpdatedOn" },
                values: new object[] { 1, new DateTime(2025, 8, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), true, "Fever", null });

            migrationBuilder.InsertData(
                table: "IllnessRules",
                columns: new[] { "IllnessId", "SymptomId", "Condition", "Days", "IsActive" },
                values: new object[] { 1, 1, 3, 2, true });

            migrationBuilder.CreateIndex(
                name: "IX_IllnessRules_SymptomId",
                table: "IllnessRules",
                column: "SymptomId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IllnessRules");

            migrationBuilder.DropTable(
                name: "Illnesses");

            migrationBuilder.DropTable(
                name: "Symptoms");
        }
    }
}
