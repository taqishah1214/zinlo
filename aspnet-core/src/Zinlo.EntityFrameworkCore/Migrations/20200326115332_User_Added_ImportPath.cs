using Microsoft.EntityFrameworkCore.Migrations;

namespace Zinlo.Migrations
{
    public partial class User_Added_ImportPath : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ImportsPaths_AbpUsers_UserId",
                table: "ImportsPaths");

            migrationBuilder.AlterColumn<long>(
                name: "UserId",
                table: "ImportsPaths",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ImportsPaths_AbpUsers_UserId",
                table: "ImportsPaths",
                column: "UserId",
                principalTable: "AbpUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ImportsPaths_AbpUsers_UserId",
                table: "ImportsPaths");

            migrationBuilder.AlterColumn<long>(
                name: "UserId",
                table: "ImportsPaths",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long));

            migrationBuilder.AddForeignKey(
                name: "FK_ImportsPaths_AbpUsers_UserId",
                table: "ImportsPaths",
                column: "UserId",
                principalTable: "AbpUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
