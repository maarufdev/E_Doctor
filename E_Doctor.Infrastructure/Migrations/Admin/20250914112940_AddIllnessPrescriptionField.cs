using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace E_Doctor.Infrastructure.Migrations.Admin
{
    /// <inheritdoc />
    public partial class AddIllnessPrescriptionField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Prescription",
                table: "Illnesses",
                type: "TEXT",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Illnesses",
                keyColumn: "Id",
                keyValue: 1,
                column: "Prescription",
                value: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Prescription",
                table: "Illnesses");
        }
    }
}
