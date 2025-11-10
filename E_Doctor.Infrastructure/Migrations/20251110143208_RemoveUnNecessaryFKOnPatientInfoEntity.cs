using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace E_Doctor.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveUnNecessaryFKOnPatientInfoEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PatientFamilyHistoryId",
                table: "PatientInformations");

            migrationBuilder.DropColumn(
                name: "PatientPastMedicalRecordId",
                table: "PatientInformations");

            migrationBuilder.DropColumn(
                name: "PatientPersonalHistoryId",
                table: "PatientInformations");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PatientFamilyHistoryId",
                table: "PatientInformations",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PatientPastMedicalRecordId",
                table: "PatientInformations",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PatientPersonalHistoryId",
                table: "PatientInformations",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
