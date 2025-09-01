using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace E_Doctor.Infrastructure.Migrations.Patient
{
    /// <inheritdoc />
    public partial class IntialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PatientIllnesRules",
                columns: table => new
                {
                    SymptomId = table.Column<int>(type: "INTEGER", nullable: false),
                    IllnessId = table.Column<int>(type: "INTEGER", nullable: false),
                    Condition = table.Column<int>(type: "INTEGER", nullable: false),
                    Days = table.Column<int>(type: "INTEGER", nullable: false),
                    Weight = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "PatientIllnesses",
                columns: table => new
                {
                    IllnessId = table.Column<int>(type: "INTEGER", nullable: false),
                    IllnessName = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "PatientSymptoms",
                columns: table => new
                {
                    SymptomId = table.Column<int>(type: "INTEGER", nullable: false),
                    SymptomName = table.Column<string>(type: "TEXT", nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PatientIllnesRules");

            migrationBuilder.DropTable(
                name: "PatientIllnesses");

            migrationBuilder.DropTable(
                name: "PatientSymptoms");
        }
    }
}
