using Microsoft.EntityFrameworkCore.Migrations;

namespace Zinlo.Migrations
{
    public partial class editon_for_specfic_customer_email : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CustomerName",
                table: "AbpEditions");

            migrationBuilder.AddColumn<string>(
                name: "CustomerEmail",
                table: "AbpEditions",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CustomerEmail",
                table: "AbpEditions");

            migrationBuilder.AddColumn<string>(
                name: "CustomerName",
                table: "AbpEditions",
                type: "text",
                nullable: true);
        }
    }
}
