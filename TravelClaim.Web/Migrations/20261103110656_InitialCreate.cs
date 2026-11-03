using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TravelClaim.Web.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TravelClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    EmployeeName = table.Column<string>(type: "TEXT", nullable: false),
                    PersonalNumber = table.Column<string>(type: "TEXT", nullable: false),
                    HomeAddress = table.Column<string>(type: "TEXT", nullable: false),
                    WorkplaceAddress = table.Column<string>(type: "TEXT", nullable: false),
                    Destination = table.Column<string>(type: "TEXT", nullable: false),
                    Purpose = table.Column<string>(type: "TEXT", nullable: false),
                    Departure = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ReturnDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    LodgingAddress = table.Column<string>(type: "TEXT", nullable: false),
                    LodgingPaidByEmployer = table.Column<bool>(type: "INTEGER", nullable: false),
                    HasOvernightStay = table.Column<bool>(type: "INTEGER", nullable: false),
                    DistanceFromHome = table.Column<decimal>(type: "TEXT", nullable: false),
                    DistanceFromWorkplace = table.Column<decimal>(type: "TEXT", nullable: false),
                    PaidAmount = table.Column<decimal>(type: "TEXT", nullable: false),
                    TotalTaxFreeAmount = table.Column<decimal>(type: "TEXT", nullable: false),
                    TaxableAmount = table.Column<decimal>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TravelClaims", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DailyCalculations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Date = table.Column<DateOnly>(type: "TEXT", nullable: false),
                    BreakfastPaid = table.Column<bool>(type: "INTEGER", nullable: false),
                    LunchPaid = table.Column<bool>(type: "INTEGER", nullable: false),
                    DinnerPaid = table.Column<bool>(type: "INTEGER", nullable: false),
                    TaxFreeAmount = table.Column<decimal>(type: "TEXT", nullable: false),
                    TaxableAmount = table.Column<decimal>(type: "TEXT", nullable: false),
                    BaseAllowance = table.Column<decimal>(type: "TEXT", nullable: false),
                    MealDeduction = table.Column<decimal>(type: "TEXT", nullable: false),
                    NightAllowance = table.Column<decimal>(type: "TEXT", nullable: false),
                    TravelClaimId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DailyCalculations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DailyCalculations_TravelClaims_TravelClaimId",
                        column: x => x.TravelClaimId,
                        principalTable: "TravelClaims",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_DailyCalculations_TravelClaimId",
                table: "DailyCalculations",
                column: "TravelClaimId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DailyCalculations");

            migrationBuilder.DropTable(
                name: "TravelClaims");
        }
    }
}
