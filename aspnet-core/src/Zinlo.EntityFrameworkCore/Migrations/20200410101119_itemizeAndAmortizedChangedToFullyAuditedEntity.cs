using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Zinlo.Migrations
{
    public partial class itemizeAndAmortizedChangedToFullyAuditedEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "DeleterUserId",
                table: "Itemizations",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletionTime",
                table: "Itemizations",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Itemizations",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModificationTime",
                table: "Itemizations",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "LastModifierUserId",
                table: "Itemizations",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "DeleterUserId",
                table: "Amortizations",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletionTime",
                table: "Amortizations",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Amortizations",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModificationTime",
                table: "Amortizations",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "LastModifierUserId",
                table: "Amortizations",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeleterUserId",
                table: "Itemizations");

            migrationBuilder.DropColumn(
                name: "DeletionTime",
                table: "Itemizations");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Itemizations");

            migrationBuilder.DropColumn(
                name: "LastModificationTime",
                table: "Itemizations");

            migrationBuilder.DropColumn(
                name: "LastModifierUserId",
                table: "Itemizations");

            migrationBuilder.DropColumn(
                name: "DeleterUserId",
                table: "Amortizations");

            migrationBuilder.DropColumn(
                name: "DeletionTime",
                table: "Amortizations");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Amortizations");

            migrationBuilder.DropColumn(
                name: "LastModificationTime",
                table: "Amortizations");

            migrationBuilder.DropColumn(
                name: "LastModifierUserId",
                table: "Amortizations");
        }
    }
}
