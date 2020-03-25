using Microsoft.EntityFrameworkCore.Migrations;

namespace Zinlo.Migrations
{
    public partial class AddedUserNavigationInImportsPath : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "UserId",
                table: "ImportsPaths",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ImportsPaths_UserId",
                table: "ImportsPaths",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_ImportsPaths_AbpUsers_UserId",
                table: "ImportsPaths",
                column: "UserId",
                principalTable: "AbpUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ImportsPaths_AbpUsers_UserId",
                table: "ImportsPaths");

            migrationBuilder.DropIndex(
                name: "IX_ImportsPaths_UserId",
                table: "ImportsPaths");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "ImportsPaths");
        }
    }
}
