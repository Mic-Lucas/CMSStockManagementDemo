using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DealershipStockManagement.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialSeed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "StockItems",
                columns: new[] { "Id", "Colour", "CostPrice", "DTCreated", "DTUpdated", "KMS", "Make", "Model", "ModelYear", "RegNo", "RetailPrice", "VIN" },
                values: new object[] { 1, "White", 180000m, new DateTime(2025, 9, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 9, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), 15000, "Toyota", "Corolla", 2020, "ABC123GP", 220000m, "1HGCM82633A004352" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "StockItems",
                keyColumn: "Id",
                keyValue: 1);
        }
    }
}
