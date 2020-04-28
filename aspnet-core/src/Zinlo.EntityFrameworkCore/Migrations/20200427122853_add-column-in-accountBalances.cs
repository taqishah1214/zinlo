using Microsoft.EntityFrameworkCore.Migrations;

namespace Zinlo.Migrations
{
    public partial class addcolumninaccountBalances : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "ChartofAccounts");

            migrationBuilder.AddColumn<bool>(
                name: "CheckAsReconcilied",
                table: "AccountBalance",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "AccountBalance",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CheckAsReconcilied",
                table: "AccountBalance");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "AccountBalance");

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "ChartofAccounts",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
