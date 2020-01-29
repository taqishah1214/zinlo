using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Zinlo.Migrations
{
    public partial class Attachments_Added : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Attachment",
                table: "ClosingChecklists");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "Attachments");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Attachments");

            migrationBuilder.AddColumn<long>(
                name: "AttachmentsId",
                table: "ClosingChecklists",
                nullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "TypeId",
                table: "Attachments",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "Attachments",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

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

        protected override void Down(MigrationBuilder migrationBuilder)
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

            migrationBuilder.AddColumn<string>(
                name: "Attachment",
                table: "ClosingChecklists",
                type: "text",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "TypeId",
                table: "Attachments",
                type: "integer",
                nullable: false,
                oldClrType: typeof(long));

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Attachments",
                type: "integer",
                nullable: false,
                oldClrType: typeof(long))
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddColumn<int>(
                name: "TenantId",
                table: "Attachments",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "Attachments",
                type: "text",
                nullable: true);
        }
    }
}
