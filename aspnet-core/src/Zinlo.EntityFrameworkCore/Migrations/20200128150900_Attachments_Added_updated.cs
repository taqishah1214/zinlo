using Microsoft.EntityFrameworkCore.Migrations;

namespace Zinlo.Migrations
{
    public partial class Attachments_Added_updated : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClosingChecklists_Attachments_AttachmentsId",
                table: "ClosingChecklists");

            migrationBuilder.DropIndex(
                name: "IX_ClosingChecklists_AttachmentsId",
                table: "ClosingChecklists");

            migrationBuilder.DropColumn(
                name: "AttachmentsId",
                table: "ClosingChecklists");

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "Attachments",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Type",
                table: "Attachments");

            migrationBuilder.AddColumn<long>(
                name: "AttachmentsId",
                table: "ClosingChecklists",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ClosingChecklists_AttachmentsId",
                table: "ClosingChecklists",
                column: "AttachmentsId");

            migrationBuilder.AddForeignKey(
                name: "FK_ClosingChecklists_Attachments_AttachmentsId",
                table: "ClosingChecklists",
                column: "AttachmentsId",
                principalTable: "Attachments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
