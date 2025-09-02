using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace E_Doctor.Infrastructure.Migrations.Patient
{
    /// <inheritdoc />
    public partial class AddedPrimaryKey : Migration
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
                    table.PrimaryKey("PK_PatientIllnesRules", x => new { x.IllnessId, x.SymptomId });
                });

            migrationBuilder.CreateTable(
                name: "PatientIllnesses",
                columns: table => new
                {
                    IllnessId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    IllnessName = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PatientIllnesses", x => x.IllnessId);
                });

            migrationBuilder.CreateTable(
                name: "PatientSymptoms",
                columns: table => new
                {
                    SymptomId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SymptomName = table.Column<string>(type: "TEXT", nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PatientSymptoms", x => x.SymptomId);
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
