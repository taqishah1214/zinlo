using Microsoft.EntityFrameworkCore.Migrations;

namespace Zinlo.Migrations
{
    public partial class AddedColumnVersionIdInChartOfAccounts : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsRollBacked",
                table: "ImportsPaths",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<long>(
                name: "VersionId",
                table: "ChartsofAccount",
                nullable: false,
                defaultValue: 0L);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsRollBacked",
                table: "ImportsPaths");

            migrationBuilder.DropColumn(
                name: "VersionId",
                table: "ChartsofAccount");
        }
    }
}
