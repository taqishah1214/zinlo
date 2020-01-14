using Microsoft.EntityFrameworkCore.Migrations;

namespace Zinlo.Migrations
{
    public partial class closingchecklist1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_AbpUsers_AssigneeNameId",
                table: "Tasks");

            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_Categories_CategoryId",
                table: "Tasks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Tasks",
                table: "Tasks");

            migrationBuilder.RenameTable(
                name: "Tasks",
                newName: "ClosingChecklists");

            migrationBuilder.RenameIndex(
                name: "IX_Tasks_CategoryId",
                table: "ClosingChecklists",
                newName: "IX_ClosingChecklists_CategoryId");

            migrationBuilder.RenameIndex(
                name: "IX_Tasks_AssigneeNameId",
                table: "ClosingChecklists",
                newName: "IX_ClosingChecklists_AssigneeNameId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ClosingChecklists",
                table: "ClosingChecklists",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ClosingChecklists_AbpUsers_AssigneeNameId",
                table: "ClosingChecklists",
                column: "AssigneeNameId",
                principalTable: "AbpUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ClosingChecklists_Categories_CategoryId",
                table: "ClosingChecklists",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClosingChecklists_AbpUsers_AssigneeNameId",
                table: "ClosingChecklists");

            migrationBuilder.DropForeignKey(
                name: "FK_ClosingChecklists_Categories_CategoryId",
                table: "ClosingChecklists");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ClosingChecklists",
                table: "ClosingChecklists");

            migrationBuilder.RenameTable(
                name: "ClosingChecklists",
                newName: "Tasks");

            migrationBuilder.RenameIndex(
                name: "IX_ClosingChecklists_CategoryId",
                table: "Tasks",
                newName: "IX_Tasks_CategoryId");

            migrationBuilder.RenameIndex(
                name: "IX_ClosingChecklists_AssigneeNameId",
                table: "Tasks",
                newName: "IX_Tasks_AssigneeNameId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Tasks",
                table: "Tasks",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_AbpUsers_AssigneeNameId",
                table: "Tasks",
                column: "AssigneeNameId",
                principalTable: "AbpUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_Categories_CategoryId",
                table: "Tasks",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
