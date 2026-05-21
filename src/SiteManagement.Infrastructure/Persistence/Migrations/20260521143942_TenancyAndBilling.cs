using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SiteManagement.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class TenancyAndBilling : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ApartmentAssignments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ApartmentId = table.Column<Guid>(type: "uuid", nullable: false),
                    ResidentId = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantType = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    Period_EndDate = table.Column<DateOnly>(type: "date", nullable: true),
                    Period_StartDate = table.Column<DateOnly>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApartmentAssignments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DuesPeriods",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SiteId = table.Column<Guid>(type: "uuid", nullable: false),
                    Month = table.Column<string>(type: "character varying(7)", maxLength: 7, nullable: false),
                    Amount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    IsClosed = table.Column<bool>(type: "boolean", nullable: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DuesPeriods", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UtilityBillPeriods",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SiteId = table.Column<Guid>(type: "uuid", nullable: false),
                    Month = table.Column<string>(type: "character varying(7)", maxLength: 7, nullable: false),
                    UtilityType = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Amount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    IsClosed = table.Column<bool>(type: "boolean", nullable: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UtilityBillPeriods", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DuesItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ApartmentId = table.Column<Guid>(type: "uuid", nullable: false),
                    ResidentId = table.Column<Guid>(type: "uuid", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    DuesPeriodId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DuesItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DuesItems_DuesPeriods_DuesPeriodId",
                        column: x => x.DuesPeriodId,
                        principalTable: "DuesPeriods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UtilityBillItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ApartmentId = table.Column<Guid>(type: "uuid", nullable: false),
                    ResidentId = table.Column<Guid>(type: "uuid", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    UtilityBillPeriodId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UtilityBillItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UtilityBillItems_UtilityBillPeriods_UtilityBillPeriodId",
                        column: x => x.UtilityBillPeriodId,
                        principalTable: "UtilityBillPeriods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApartmentAssignments_ApartmentId",
                table: "ApartmentAssignments",
                column: "ApartmentId",
                unique: true,
                filter: "\"Period_EndDate\" IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_DuesItems_DuesPeriodId_ApartmentId",
                table: "DuesItems",
                columns: new[] { "DuesPeriodId", "ApartmentId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UtilityBillItems_UtilityBillPeriodId_ApartmentId",
                table: "UtilityBillItems",
                columns: new[] { "UtilityBillPeriodId", "ApartmentId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApartmentAssignments");

            migrationBuilder.DropTable(
                name: "DuesItems");

            migrationBuilder.DropTable(
                name: "UtilityBillItems");

            migrationBuilder.DropTable(
                name: "DuesPeriods");

            migrationBuilder.DropTable(
                name: "UtilityBillPeriods");
        }
    }
}
