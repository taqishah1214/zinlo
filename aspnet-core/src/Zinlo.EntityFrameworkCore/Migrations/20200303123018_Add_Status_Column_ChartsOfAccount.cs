using Microsoft.EntityFrameworkCore.Migrations;

namespace Zinlo.Migrations
{
    public partial class Add_Status_Column_ChartsOfAccount : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "ChartsofAccount",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "ChartsofAccount");
        }
    }
}
