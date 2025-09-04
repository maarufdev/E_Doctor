using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace E_Doctor.Infrastructure.Migrations.Admin
{
    /// <inheritdoc />
    public partial class AddedDiganosisEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Diagnosis",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    IllnessName = table.Column<string>(type: "TEXT", nullable: true),
                    Symptoms = table.Column<string>(type: "TEXT", nullable: true),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "TEXT", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Diagnosis", x => x.Id);
                });

            migrationBuilder.UpdateData(
                table: "IllnessRules",
                keyColumns: new[] { "IllnessId", "SymptomId" },
                keyValues: new object[] { 1, 1 },
                column: "Condition",
                value: 1);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Diagnosis");

            migrationBuilder.UpdateData(
                table: "IllnessRules",
                keyColumns: new[] { "IllnessId", "SymptomId" },
                keyValues: new object[] { 1, 1 },
                column: "Condition",
                value: 3);
        }
    }
}
