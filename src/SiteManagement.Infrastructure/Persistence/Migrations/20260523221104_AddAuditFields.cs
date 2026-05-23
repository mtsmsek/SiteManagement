using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SiteManagement.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddAuditFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAtUtc",
                table: "UtilityBillPeriods",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedBy",
                table: "UtilityBillPeriods",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedAtUtc",
                table: "UtilityBillPeriods",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ModifiedBy",
                table: "UtilityBillPeriods",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAtUtc",
                table: "Sites",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedBy",
                table: "Sites",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedAtUtc",
                table: "Sites",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ModifiedBy",
                table: "Sites",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAtUtc",
                table: "Residents",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedBy",
                table: "Residents",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedAtUtc",
                table: "Residents",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ModifiedBy",
                table: "Residents",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAtUtc",
                table: "DuesPeriods",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedBy",
                table: "DuesPeriods",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedAtUtc",
                table: "DuesPeriods",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ModifiedBy",
                table: "DuesPeriods",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAtUtc",
                table: "ApartmentAssignments",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedBy",
                table: "ApartmentAssignments",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedAtUtc",
                table: "ApartmentAssignments",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ModifiedBy",
                table: "ApartmentAssignments",
                type: "uuid",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAtUtc",
                table: "UtilityBillPeriods");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "UtilityBillPeriods");

            migrationBuilder.DropColumn(
                name: "ModifiedAtUtc",
                table: "UtilityBillPeriods");

            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                table: "UtilityBillPeriods");

            migrationBuilder.DropColumn(
                name: "CreatedAtUtc",
                table: "Sites");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Sites");

            migrationBuilder.DropColumn(
                name: "ModifiedAtUtc",
                table: "Sites");

            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                table: "Sites");

            migrationBuilder.DropColumn(
                name: "CreatedAtUtc",
                table: "Residents");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Residents");

            migrationBuilder.DropColumn(
                name: "ModifiedAtUtc",
                table: "Residents");

            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                table: "Residents");

            migrationBuilder.DropColumn(
                name: "CreatedAtUtc",
                table: "DuesPeriods");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "DuesPeriods");

            migrationBuilder.DropColumn(
                name: "ModifiedAtUtc",
                table: "DuesPeriods");

            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                table: "DuesPeriods");

            migrationBuilder.DropColumn(
                name: "CreatedAtUtc",
                table: "ApartmentAssignments");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "ApartmentAssignments");

            migrationBuilder.DropColumn(
                name: "ModifiedAtUtc",
                table: "ApartmentAssignments");

            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                table: "ApartmentAssignments");
        }
    }
}
