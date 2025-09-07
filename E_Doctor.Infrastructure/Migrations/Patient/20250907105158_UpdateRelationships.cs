using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace E_Doctor.Infrastructure.Migrations.Patient
{
    /// <inheritdoc />
    public partial class UpdateRelationships : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_PatientDiagnosis_UserId",
                table: "PatientDiagnosis",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_PatientDiagnosis_AspNetUsers_UserId",
                table: "PatientDiagnosis",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PatientDiagnosis_AspNetUsers_UserId",
                table: "PatientDiagnosis");

            migrationBuilder.DropIndex(
                name: "IX_PatientDiagnosis_UserId",
                table: "PatientDiagnosis");
        }
    }
}
