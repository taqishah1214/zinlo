using Microsoft.EntityFrameworkCore.Migrations;

namespace Zinlo.Migrations
{
    public partial class Closing_Checklist_Assignee_Correction : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClosingChecklists_AbpUsers_AssigneeNameId",
                table: "ClosingChecklists");

            migrationBuilder.DropIndex(
                name: "IX_ClosingChecklists_AssigneeNameId",
                table: "ClosingChecklists");

            migrationBuilder.DropColumn(
                name: "AssigneeNameId",
                table: "ClosingChecklists");

            migrationBuilder.AddColumn<long>(
                name: "AssigneeId",
                table: "ClosingChecklists",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_ClosingChecklists_AssigneeId",
                table: "ClosingChecklists",
                column: "AssigneeId");

            migrationBuilder.AddForeignKey(
                name: "FK_ClosingChecklists_AbpUsers_AssigneeId",
                table: "ClosingChecklists",
                column: "AssigneeId",
                principalTable: "AbpUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClosingChecklists_AbpUsers_AssigneeId",
                table: "ClosingChecklists");

            migrationBuilder.DropIndex(
                name: "IX_ClosingChecklists_AssigneeId",
                table: "ClosingChecklists");

            migrationBuilder.DropColumn(
                name: "AssigneeId",
                table: "ClosingChecklists");

            migrationBuilder.AddColumn<long>(
                name: "AssigneeNameId",
                table: "ClosingChecklists",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_ClosingChecklists_AssigneeNameId",
                table: "ClosingChecklists",
                column: "AssigneeNameId");

            migrationBuilder.AddForeignKey(
                name: "FK_ClosingChecklists_AbpUsers_AssigneeNameId",
                table: "ClosingChecklists",
                column: "AssigneeNameId",
                principalTable: "AbpUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
