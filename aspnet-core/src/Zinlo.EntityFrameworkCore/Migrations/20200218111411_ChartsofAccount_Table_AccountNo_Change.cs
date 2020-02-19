using Microsoft.EntityFrameworkCore.Migrations;

namespace Zinlo.Migrations
{
    public partial class ChartsofAccount_Table_AccountNo_Change : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AccountNo",
                table: "ChartsofAccount");

            migrationBuilder.AddColumn<string>(
                name: "AccountNumber",
                table: "ChartsofAccount",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AccountNumber",
                table: "ChartsofAccount");

            migrationBuilder.AddColumn<long>(
                name: "AccountNo",
                table: "ChartsofAccount",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }
    }
}
