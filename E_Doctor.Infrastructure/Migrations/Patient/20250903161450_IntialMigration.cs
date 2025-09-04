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
                name: "PatientDiagnosis",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<int>(type: "INTEGER", nullable: true),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "TEXT", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PatientDiagnosis", x => x.Id);
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

            migrationBuilder.CreateTable(
                name: "PatientDiagnosisIllnesses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DiagnosisId = table.Column<int>(type: "INTEGER", nullable: false),
                    Illness = table.Column<string>(type: "TEXT", nullable: false),
                    Score = table.Column<decimal>(type: "TEXT", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "TEXT", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PatientDiagnosisIllnesses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PatientDiagnosisIllnesses_PatientDiagnosis_DiagnosisId",
                        column: x => x.DiagnosisId,
                        principalTable: "PatientDiagnosis",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PatientDiagnosisSymptoms",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DiagnosisId = table.Column<int>(type: "INTEGER", nullable: false),
                    SymptomName = table.Column<string>(type: "TEXT", nullable: true),
                    Days = table.Column<int>(type: "INTEGER", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "TEXT", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PatientDiagnosisSymptoms", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PatientDiagnosisSymptoms_PatientDiagnosis_DiagnosisId",
                        column: x => x.DiagnosisId,
                        principalTable: "PatientDiagnosis",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

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
                    table.ForeignKey(
                        name: "FK_PatientIllnesRules_PatientIllnesses_IllnessId",
                        column: x => x.IllnessId,
                        principalTable: "PatientIllnesses",
                        principalColumn: "IllnessId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PatientIllnesRules_PatientSymptoms_SymptomId",
                        column: x => x.SymptomId,
                        principalTable: "PatientSymptoms",
                        principalColumn: "SymptomId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PatientDiagnosisIllnesses_DiagnosisId",
                table: "PatientDiagnosisIllnesses",
                column: "DiagnosisId");

            migrationBuilder.CreateIndex(
                name: "IX_PatientDiagnosisSymptoms_DiagnosisId",
                table: "PatientDiagnosisSymptoms",
                column: "DiagnosisId");

            migrationBuilder.CreateIndex(
                name: "IX_PatientIllnesRules_SymptomId",
                table: "PatientIllnesRules",
                column: "SymptomId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PatientDiagnosisIllnesses");

            migrationBuilder.DropTable(
                name: "PatientDiagnosisSymptoms");

            migrationBuilder.DropTable(
                name: "PatientIllnesRules");

            migrationBuilder.DropTable(
                name: "PatientDiagnosis");

            migrationBuilder.DropTable(
                name: "PatientIllnesses");

            migrationBuilder.DropTable(
                name: "PatientSymptoms");
        }
    }
}
