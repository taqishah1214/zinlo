using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Zinlo.Migrations
{
    public partial class add_column_chartsofaccounts : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "VersionId",
                table: "ChartofAccounts");

            migrationBuilder.AddColumn<DateTime>(
                name: "ChangeTime",
                table: "ChartofAccounts",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsChange",
                table: "ChartofAccounts",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ChangeTime",
                table: "ChartofAccounts");

            migrationBuilder.DropColumn(
                name: "IsChange",
                table: "ChartofAccounts");

            migrationBuilder.AddColumn<long>(
                name: "VersionId",
                table: "ChartofAccounts",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }
    }
}
