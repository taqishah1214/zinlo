using Microsoft.EntityFrameworkCore.Migrations;

namespace Zinlo.Migrations
{
    public partial class add_linked_column : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LinkedAccountNumber",
                table: "ChartofAccounts",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LinkedAccountNumber",
                table: "ChartofAccounts");
        }
    }
}
