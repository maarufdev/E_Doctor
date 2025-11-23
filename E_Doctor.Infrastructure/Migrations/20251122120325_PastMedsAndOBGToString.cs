using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace E_Doctor.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class PastMedsAndOBGToString : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "OBGyneHistory",
                table: "PatientPastMedicalRecords",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<string>(
                name: "MaintenanceMeds",
                table: "PatientPastMedicalRecords",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.UpdateData(
                table: "PhysicalExamItems",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedOn",
                value: new DateTime(2025, 11, 22, 12, 3, 24, 280, DateTimeKind.Utc).AddTicks(1478));

            migrationBuilder.UpdateData(
                table: "PhysicalExamItems",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedOn",
                value: new DateTime(2025, 11, 22, 12, 3, 24, 280, DateTimeKind.Utc).AddTicks(1478));

            migrationBuilder.UpdateData(
                table: "PhysicalExamItems",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedOn",
                value: new DateTime(2025, 11, 22, 12, 3, 24, 280, DateTimeKind.Utc).AddTicks(1478));

            migrationBuilder.UpdateData(
                table: "PhysicalExamItems",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedOn",
                value: new DateTime(2025, 11, 22, 12, 3, 24, 280, DateTimeKind.Utc).AddTicks(1478));

            migrationBuilder.UpdateData(
                table: "PhysicalExamItems",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedOn",
                value: new DateTime(2025, 11, 22, 12, 3, 24, 280, DateTimeKind.Utc).AddTicks(1478));

            migrationBuilder.UpdateData(
                table: "PhysicalExamItems",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedOn",
                value: new DateTime(2025, 11, 22, 12, 3, 24, 280, DateTimeKind.Utc).AddTicks(1478));

            migrationBuilder.UpdateData(
                table: "PhysicalExamItems",
                keyColumn: "Id",
                keyValue: 7,
                column: "CreatedOn",
                value: new DateTime(2025, 11, 22, 12, 3, 24, 280, DateTimeKind.Utc).AddTicks(1478));

            migrationBuilder.UpdateData(
                table: "PhysicalExamItems",
                keyColumn: "Id",
                keyValue: 8,
                column: "CreatedOn",
                value: new DateTime(2025, 11, 22, 12, 3, 24, 280, DateTimeKind.Utc).AddTicks(1478));

            migrationBuilder.UpdateData(
                table: "PhysicalExamItems",
                keyColumn: "Id",
                keyValue: 9,
                column: "CreatedOn",
                value: new DateTime(2025, 11, 22, 12, 3, 24, 280, DateTimeKind.Utc).AddTicks(1478));

            migrationBuilder.UpdateData(
                table: "PhysicalExamItems",
                keyColumn: "Id",
                keyValue: 10,
                column: "CreatedOn",
                value: new DateTime(2025, 11, 22, 12, 3, 24, 280, DateTimeKind.Utc).AddTicks(1478));

            migrationBuilder.UpdateData(
                table: "PhysicalExamItems",
                keyColumn: "Id",
                keyValue: 11,
                column: "CreatedOn",
                value: new DateTime(2025, 11, 22, 12, 3, 24, 280, DateTimeKind.Utc).AddTicks(1478));

            migrationBuilder.UpdateData(
                table: "PhysicalExamItems",
                keyColumn: "Id",
                keyValue: 12,
                column: "CreatedOn",
                value: new DateTime(2025, 11, 22, 12, 3, 24, 280, DateTimeKind.Utc).AddTicks(1478));

            migrationBuilder.UpdateData(
                table: "PhysicalExamItems",
                keyColumn: "Id",
                keyValue: 13,
                column: "CreatedOn",
                value: new DateTime(2025, 11, 22, 12, 3, 24, 280, DateTimeKind.Utc).AddTicks(1478));

            migrationBuilder.UpdateData(
                table: "PhysicalExamItems",
                keyColumn: "Id",
                keyValue: 14,
                column: "CreatedOn",
                value: new DateTime(2025, 11, 22, 12, 3, 24, 280, DateTimeKind.Utc).AddTicks(1478));

            migrationBuilder.UpdateData(
                table: "PhysicalExamItems",
                keyColumn: "Id",
                keyValue: 15,
                column: "CreatedOn",
                value: new DateTime(2025, 11, 22, 12, 3, 24, 280, DateTimeKind.Utc).AddTicks(1478));

            migrationBuilder.UpdateData(
                table: "PhysicalExamItems",
                keyColumn: "Id",
                keyValue: 16,
                column: "CreatedOn",
                value: new DateTime(2025, 11, 22, 12, 3, 24, 280, DateTimeKind.Utc).AddTicks(1478));

            migrationBuilder.UpdateData(
                table: "PhysicalExamItems",
                keyColumn: "Id",
                keyValue: 17,
                column: "CreatedOn",
                value: new DateTime(2025, 11, 22, 12, 3, 24, 280, DateTimeKind.Utc).AddTicks(1478));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "OBGyneHistory",
                table: "PatientPastMedicalRecords",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "MaintenanceMeds",
                table: "PatientPastMedicalRecords",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "PhysicalExamItems",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedOn",
                value: new DateTime(2025, 11, 22, 11, 43, 30, 877, DateTimeKind.Utc).AddTicks(9535));

            migrationBuilder.UpdateData(
                table: "PhysicalExamItems",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedOn",
                value: new DateTime(2025, 11, 22, 11, 43, 30, 877, DateTimeKind.Utc).AddTicks(9535));

            migrationBuilder.UpdateData(
                table: "PhysicalExamItems",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedOn",
                value: new DateTime(2025, 11, 22, 11, 43, 30, 877, DateTimeKind.Utc).AddTicks(9535));

            migrationBuilder.UpdateData(
                table: "PhysicalExamItems",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedOn",
                value: new DateTime(2025, 11, 22, 11, 43, 30, 877, DateTimeKind.Utc).AddTicks(9535));

            migrationBuilder.UpdateData(
                table: "PhysicalExamItems",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedOn",
                value: new DateTime(2025, 11, 22, 11, 43, 30, 877, DateTimeKind.Utc).AddTicks(9535));

            migrationBuilder.UpdateData(
                table: "PhysicalExamItems",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedOn",
                value: new DateTime(2025, 11, 22, 11, 43, 30, 877, DateTimeKind.Utc).AddTicks(9535));

            migrationBuilder.UpdateData(
                table: "PhysicalExamItems",
                keyColumn: "Id",
                keyValue: 7,
                column: "CreatedOn",
                value: new DateTime(2025, 11, 22, 11, 43, 30, 877, DateTimeKind.Utc).AddTicks(9535));

            migrationBuilder.UpdateData(
                table: "PhysicalExamItems",
                keyColumn: "Id",
                keyValue: 8,
                column: "CreatedOn",
                value: new DateTime(2025, 11, 22, 11, 43, 30, 877, DateTimeKind.Utc).AddTicks(9535));

            migrationBuilder.UpdateData(
                table: "PhysicalExamItems",
                keyColumn: "Id",
                keyValue: 9,
                column: "CreatedOn",
                value: new DateTime(2025, 11, 22, 11, 43, 30, 877, DateTimeKind.Utc).AddTicks(9535));

            migrationBuilder.UpdateData(
                table: "PhysicalExamItems",
                keyColumn: "Id",
                keyValue: 10,
                column: "CreatedOn",
                value: new DateTime(2025, 11, 22, 11, 43, 30, 877, DateTimeKind.Utc).AddTicks(9535));

            migrationBuilder.UpdateData(
                table: "PhysicalExamItems",
                keyColumn: "Id",
                keyValue: 11,
                column: "CreatedOn",
                value: new DateTime(2025, 11, 22, 11, 43, 30, 877, DateTimeKind.Utc).AddTicks(9535));

            migrationBuilder.UpdateData(
                table: "PhysicalExamItems",
                keyColumn: "Id",
                keyValue: 12,
                column: "CreatedOn",
                value: new DateTime(2025, 11, 22, 11, 43, 30, 877, DateTimeKind.Utc).AddTicks(9535));

            migrationBuilder.UpdateData(
                table: "PhysicalExamItems",
                keyColumn: "Id",
                keyValue: 13,
                column: "CreatedOn",
                value: new DateTime(2025, 11, 22, 11, 43, 30, 877, DateTimeKind.Utc).AddTicks(9535));

            migrationBuilder.UpdateData(
                table: "PhysicalExamItems",
                keyColumn: "Id",
                keyValue: 14,
                column: "CreatedOn",
                value: new DateTime(2025, 11, 22, 11, 43, 30, 877, DateTimeKind.Utc).AddTicks(9535));

            migrationBuilder.UpdateData(
                table: "PhysicalExamItems",
                keyColumn: "Id",
                keyValue: 15,
                column: "CreatedOn",
                value: new DateTime(2025, 11, 22, 11, 43, 30, 877, DateTimeKind.Utc).AddTicks(9535));

            migrationBuilder.UpdateData(
                table: "PhysicalExamItems",
                keyColumn: "Id",
                keyValue: 16,
                column: "CreatedOn",
                value: new DateTime(2025, 11, 22, 11, 43, 30, 877, DateTimeKind.Utc).AddTicks(9535));

            migrationBuilder.UpdateData(
                table: "PhysicalExamItems",
                keyColumn: "Id",
                keyValue: 17,
                column: "CreatedOn",
                value: new DateTime(2025, 11, 22, 11, 43, 30, 877, DateTimeKind.Utc).AddTicks(9535));
        }
    }
}
