using Microsoft.EntityFrameworkCore.Migrations;

namespace Zinlo.Migrations
{
    public partial class columnsAddedInImportPaths : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FailedRecordsCount",
                table: "ImportsPaths",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SuccessRecordsCount",
                table: "ImportsPaths",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "ImportsPaths",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FailedRecordsCount",
                table: "ImportsPaths");

            migrationBuilder.DropColumn(
                name: "SuccessRecordsCount",
                table: "ImportsPaths");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "ImportsPaths");
        }
    }
}
