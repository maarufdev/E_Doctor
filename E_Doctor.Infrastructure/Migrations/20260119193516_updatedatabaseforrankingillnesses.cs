using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace E_Doctor.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class updatedatabaseforrankingillnesses : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SymptomId",
                table: "DiagnosisSymptoms",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ExpectedRulesCount",
                table: "DiagnosisIllnesses",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MatchedRulesCount",
                table: "DiagnosisIllnesses",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "PhysicalExamItems",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedOn",
                value: new DateTime(2026, 1, 19, 19, 35, 16, 438, DateTimeKind.Utc).AddTicks(8926));

            migrationBuilder.UpdateData(
                table: "PhysicalExamItems",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedOn",
                value: new DateTime(2026, 1, 19, 19, 35, 16, 438, DateTimeKind.Utc).AddTicks(8926));

            migrationBuilder.UpdateData(
                table: "PhysicalExamItems",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedOn",
                value: new DateTime(2026, 1, 19, 19, 35, 16, 438, DateTimeKind.Utc).AddTicks(8926));

            migrationBuilder.UpdateData(
                table: "PhysicalExamItems",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedOn",
                value: new DateTime(2026, 1, 19, 19, 35, 16, 438, DateTimeKind.Utc).AddTicks(8926));

            migrationBuilder.UpdateData(
                table: "PhysicalExamItems",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedOn",
                value: new DateTime(2026, 1, 19, 19, 35, 16, 438, DateTimeKind.Utc).AddTicks(8926));

            migrationBuilder.UpdateData(
                table: "PhysicalExamItems",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedOn",
                value: new DateTime(2026, 1, 19, 19, 35, 16, 438, DateTimeKind.Utc).AddTicks(8926));

            migrationBuilder.UpdateData(
                table: "PhysicalExamItems",
                keyColumn: "Id",
                keyValue: 7,
                column: "CreatedOn",
                value: new DateTime(2026, 1, 19, 19, 35, 16, 438, DateTimeKind.Utc).AddTicks(8926));

            migrationBuilder.UpdateData(
                table: "PhysicalExamItems",
                keyColumn: "Id",
                keyValue: 8,
                column: "CreatedOn",
                value: new DateTime(2026, 1, 19, 19, 35, 16, 438, DateTimeKind.Utc).AddTicks(8926));

            migrationBuilder.UpdateData(
                table: "PhysicalExamItems",
                keyColumn: "Id",
                keyValue: 9,
                column: "CreatedOn",
                value: new DateTime(2026, 1, 19, 19, 35, 16, 438, DateTimeKind.Utc).AddTicks(8926));

            migrationBuilder.UpdateData(
                table: "PhysicalExamItems",
                keyColumn: "Id",
                keyValue: 10,
                column: "CreatedOn",
                value: new DateTime(2026, 1, 19, 19, 35, 16, 438, DateTimeKind.Utc).AddTicks(8926));

            migrationBuilder.UpdateData(
                table: "PhysicalExamItems",
                keyColumn: "Id",
                keyValue: 11,
                column: "CreatedOn",
                value: new DateTime(2026, 1, 19, 19, 35, 16, 438, DateTimeKind.Utc).AddTicks(8926));

            migrationBuilder.UpdateData(
                table: "PhysicalExamItems",
                keyColumn: "Id",
                keyValue: 12,
                column: "CreatedOn",
                value: new DateTime(2026, 1, 19, 19, 35, 16, 438, DateTimeKind.Utc).AddTicks(8926));

            migrationBuilder.UpdateData(
                table: "PhysicalExamItems",
                keyColumn: "Id",
                keyValue: 13,
                column: "CreatedOn",
                value: new DateTime(2026, 1, 19, 19, 35, 16, 438, DateTimeKind.Utc).AddTicks(8926));

            migrationBuilder.UpdateData(
                table: "PhysicalExamItems",
                keyColumn: "Id",
                keyValue: 14,
                column: "CreatedOn",
                value: new DateTime(2026, 1, 19, 19, 35, 16, 438, DateTimeKind.Utc).AddTicks(8926));

            migrationBuilder.UpdateData(
                table: "PhysicalExamItems",
                keyColumn: "Id",
                keyValue: 15,
                column: "CreatedOn",
                value: new DateTime(2026, 1, 19, 19, 35, 16, 438, DateTimeKind.Utc).AddTicks(8926));

            migrationBuilder.UpdateData(
                table: "PhysicalExamItems",
                keyColumn: "Id",
                keyValue: 16,
                column: "CreatedOn",
                value: new DateTime(2026, 1, 19, 19, 35, 16, 438, DateTimeKind.Utc).AddTicks(8926));

            migrationBuilder.UpdateData(
                table: "PhysicalExamItems",
                keyColumn: "Id",
                keyValue: 17,
                column: "CreatedOn",
                value: new DateTime(2026, 1, 19, 19, 35, 16, 438, DateTimeKind.Utc).AddTicks(8926));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SymptomId",
                table: "DiagnosisSymptoms");

            migrationBuilder.DropColumn(
                name: "ExpectedRulesCount",
                table: "DiagnosisIllnesses");

            migrationBuilder.DropColumn(
                name: "MatchedRulesCount",
                table: "DiagnosisIllnesses");

            migrationBuilder.UpdateData(
                table: "PhysicalExamItems",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedOn",
                value: new DateTime(2026, 1, 18, 11, 57, 8, 121, DateTimeKind.Utc).AddTicks(7607));

            migrationBuilder.UpdateData(
                table: "PhysicalExamItems",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedOn",
                value: new DateTime(2026, 1, 18, 11, 57, 8, 121, DateTimeKind.Utc).AddTicks(7607));

            migrationBuilder.UpdateData(
                table: "PhysicalExamItems",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedOn",
                value: new DateTime(2026, 1, 18, 11, 57, 8, 121, DateTimeKind.Utc).AddTicks(7607));

            migrationBuilder.UpdateData(
                table: "PhysicalExamItems",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedOn",
                value: new DateTime(2026, 1, 18, 11, 57, 8, 121, DateTimeKind.Utc).AddTicks(7607));

            migrationBuilder.UpdateData(
                table: "PhysicalExamItems",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedOn",
                value: new DateTime(2026, 1, 18, 11, 57, 8, 121, DateTimeKind.Utc).AddTicks(7607));

            migrationBuilder.UpdateData(
                table: "PhysicalExamItems",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedOn",
                value: new DateTime(2026, 1, 18, 11, 57, 8, 121, DateTimeKind.Utc).AddTicks(7607));

            migrationBuilder.UpdateData(
                table: "PhysicalExamItems",
                keyColumn: "Id",
                keyValue: 7,
                column: "CreatedOn",
                value: new DateTime(2026, 1, 18, 11, 57, 8, 121, DateTimeKind.Utc).AddTicks(7607));

            migrationBuilder.UpdateData(
                table: "PhysicalExamItems",
                keyColumn: "Id",
                keyValue: 8,
                column: "CreatedOn",
                value: new DateTime(2026, 1, 18, 11, 57, 8, 121, DateTimeKind.Utc).AddTicks(7607));

            migrationBuilder.UpdateData(
                table: "PhysicalExamItems",
                keyColumn: "Id",
                keyValue: 9,
                column: "CreatedOn",
                value: new DateTime(2026, 1, 18, 11, 57, 8, 121, DateTimeKind.Utc).AddTicks(7607));

            migrationBuilder.UpdateData(
                table: "PhysicalExamItems",
                keyColumn: "Id",
                keyValue: 10,
                column: "CreatedOn",
                value: new DateTime(2026, 1, 18, 11, 57, 8, 121, DateTimeKind.Utc).AddTicks(7607));

            migrationBuilder.UpdateData(
                table: "PhysicalExamItems",
                keyColumn: "Id",
                keyValue: 11,
                column: "CreatedOn",
                value: new DateTime(2026, 1, 18, 11, 57, 8, 121, DateTimeKind.Utc).AddTicks(7607));

            migrationBuilder.UpdateData(
                table: "PhysicalExamItems",
                keyColumn: "Id",
                keyValue: 12,
                column: "CreatedOn",
                value: new DateTime(2026, 1, 18, 11, 57, 8, 121, DateTimeKind.Utc).AddTicks(7607));

            migrationBuilder.UpdateData(
                table: "PhysicalExamItems",
                keyColumn: "Id",
                keyValue: 13,
                column: "CreatedOn",
                value: new DateTime(2026, 1, 18, 11, 57, 8, 121, DateTimeKind.Utc).AddTicks(7607));

            migrationBuilder.UpdateData(
                table: "PhysicalExamItems",
                keyColumn: "Id",
                keyValue: 14,
                column: "CreatedOn",
                value: new DateTime(2026, 1, 18, 11, 57, 8, 121, DateTimeKind.Utc).AddTicks(7607));

            migrationBuilder.UpdateData(
                table: "PhysicalExamItems",
                keyColumn: "Id",
                keyValue: 15,
                column: "CreatedOn",
                value: new DateTime(2026, 1, 18, 11, 57, 8, 121, DateTimeKind.Utc).AddTicks(7607));

            migrationBuilder.UpdateData(
                table: "PhysicalExamItems",
                keyColumn: "Id",
                keyValue: 16,
                column: "CreatedOn",
                value: new DateTime(2026, 1, 18, 11, 57, 8, 121, DateTimeKind.Utc).AddTicks(7607));

            migrationBuilder.UpdateData(
                table: "PhysicalExamItems",
                keyColumn: "Id",
                keyValue: 17,
                column: "CreatedOn",
                value: new DateTime(2026, 1, 18, 11, 57, 8, 121, DateTimeKind.Utc).AddTicks(7607));
        }
    }
}
