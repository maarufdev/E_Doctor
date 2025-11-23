using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace E_Doctor.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MakePastMedicalRecordsBooleanTypes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "PreviousHospitalization",
                table: "PatientPastMedicalRecords",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "PastSurgery",
                table: "PatientPastMedicalRecords",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "OtherIllnesses",
                table: "PatientPastMedicalRecords",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

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

            migrationBuilder.AlterColumn<bool>(
                name: "Hypertension",
                table: "PatientPastMedicalRecords",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "HeartProblem",
                table: "PatientPastMedicalRecords",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "FoodAllergies",
                table: "PatientPastMedicalRecords",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "Diabetes",
                table: "PatientPastMedicalRecords",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "Cancer",
                table: "PatientPastMedicalRecords",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "Asthma",
                table: "PatientPastMedicalRecords",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "AllergyToMeds",
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "PreviousHospitalization",
                table: "PatientPastMedicalRecords",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<string>(
                name: "PastSurgery",
                table: "PatientPastMedicalRecords",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<string>(
                name: "OtherIllnesses",
                table: "PatientPastMedicalRecords",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

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

            migrationBuilder.AlterColumn<string>(
                name: "Hypertension",
                table: "PatientPastMedicalRecords",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<string>(
                name: "HeartProblem",
                table: "PatientPastMedicalRecords",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<string>(
                name: "FoodAllergies",
                table: "PatientPastMedicalRecords",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<string>(
                name: "Diabetes",
                table: "PatientPastMedicalRecords",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<string>(
                name: "Cancer",
                table: "PatientPastMedicalRecords",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<string>(
                name: "Asthma",
                table: "PatientPastMedicalRecords",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<string>(
                name: "AllergyToMeds",
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
                value: new DateTime(2025, 11, 17, 15, 14, 25, 948, DateTimeKind.Utc).AddTicks(9593));

            migrationBuilder.UpdateData(
                table: "PhysicalExamItems",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedOn",
                value: new DateTime(2025, 11, 17, 15, 14, 25, 948, DateTimeKind.Utc).AddTicks(9593));

            migrationBuilder.UpdateData(
                table: "PhysicalExamItems",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedOn",
                value: new DateTime(2025, 11, 17, 15, 14, 25, 948, DateTimeKind.Utc).AddTicks(9593));

            migrationBuilder.UpdateData(
                table: "PhysicalExamItems",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedOn",
                value: new DateTime(2025, 11, 17, 15, 14, 25, 948, DateTimeKind.Utc).AddTicks(9593));

            migrationBuilder.UpdateData(
                table: "PhysicalExamItems",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedOn",
                value: new DateTime(2025, 11, 17, 15, 14, 25, 948, DateTimeKind.Utc).AddTicks(9593));

            migrationBuilder.UpdateData(
                table: "PhysicalExamItems",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedOn",
                value: new DateTime(2025, 11, 17, 15, 14, 25, 948, DateTimeKind.Utc).AddTicks(9593));

            migrationBuilder.UpdateData(
                table: "PhysicalExamItems",
                keyColumn: "Id",
                keyValue: 7,
                column: "CreatedOn",
                value: new DateTime(2025, 11, 17, 15, 14, 25, 948, DateTimeKind.Utc).AddTicks(9593));

            migrationBuilder.UpdateData(
                table: "PhysicalExamItems",
                keyColumn: "Id",
                keyValue: 8,
                column: "CreatedOn",
                value: new DateTime(2025, 11, 17, 15, 14, 25, 948, DateTimeKind.Utc).AddTicks(9593));

            migrationBuilder.UpdateData(
                table: "PhysicalExamItems",
                keyColumn: "Id",
                keyValue: 9,
                column: "CreatedOn",
                value: new DateTime(2025, 11, 17, 15, 14, 25, 948, DateTimeKind.Utc).AddTicks(9593));

            migrationBuilder.UpdateData(
                table: "PhysicalExamItems",
                keyColumn: "Id",
                keyValue: 10,
                column: "CreatedOn",
                value: new DateTime(2025, 11, 17, 15, 14, 25, 948, DateTimeKind.Utc).AddTicks(9593));

            migrationBuilder.UpdateData(
                table: "PhysicalExamItems",
                keyColumn: "Id",
                keyValue: 11,
                column: "CreatedOn",
                value: new DateTime(2025, 11, 17, 15, 14, 25, 948, DateTimeKind.Utc).AddTicks(9593));

            migrationBuilder.UpdateData(
                table: "PhysicalExamItems",
                keyColumn: "Id",
                keyValue: 12,
                column: "CreatedOn",
                value: new DateTime(2025, 11, 17, 15, 14, 25, 948, DateTimeKind.Utc).AddTicks(9593));

            migrationBuilder.UpdateData(
                table: "PhysicalExamItems",
                keyColumn: "Id",
                keyValue: 13,
                column: "CreatedOn",
                value: new DateTime(2025, 11, 17, 15, 14, 25, 948, DateTimeKind.Utc).AddTicks(9593));

            migrationBuilder.UpdateData(
                table: "PhysicalExamItems",
                keyColumn: "Id",
                keyValue: 14,
                column: "CreatedOn",
                value: new DateTime(2025, 11, 17, 15, 14, 25, 948, DateTimeKind.Utc).AddTicks(9593));

            migrationBuilder.UpdateData(
                table: "PhysicalExamItems",
                keyColumn: "Id",
                keyValue: 15,
                column: "CreatedOn",
                value: new DateTime(2025, 11, 17, 15, 14, 25, 948, DateTimeKind.Utc).AddTicks(9593));

            migrationBuilder.UpdateData(
                table: "PhysicalExamItems",
                keyColumn: "Id",
                keyValue: 16,
                column: "CreatedOn",
                value: new DateTime(2025, 11, 17, 15, 14, 25, 948, DateTimeKind.Utc).AddTicks(9593));

            migrationBuilder.UpdateData(
                table: "PhysicalExamItems",
                keyColumn: "Id",
                keyValue: 17,
                column: "CreatedOn",
                value: new DateTime(2025, 11, 17, 15, 14, 25, 948, DateTimeKind.Utc).AddTicks(9593));
        }
    }
}
