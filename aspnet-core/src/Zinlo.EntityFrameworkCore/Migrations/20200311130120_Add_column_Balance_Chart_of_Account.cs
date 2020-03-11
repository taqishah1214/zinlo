using Microsoft.EntityFrameworkCore.Migrations;

namespace Zinlo.Migrations
{
    public partial class Add_column_Balance_Chart_of_Account : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Balance",
                table: "ChartsofAccount",
                nullable: false,
                defaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Balance",
                table: "ChartsofAccount");
        }
    }
}
