using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Zinlo.Migrations
{
    public partial class add_Column_chartsofaccount_itemized_amortized : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ClosingMonth",
                table: "Itemizations",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "ClosingMonth",
                table: "ChartsofAccount",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "ClosingMonth",
                table: "Amortizations",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ClosingMonth",
                table: "Itemizations");

            migrationBuilder.DropColumn(
                name: "ClosingMonth",
                table: "ChartsofAccount");

            migrationBuilder.DropColumn(
                name: "ClosingMonth",
                table: "Amortizations");
        }
    }
}
