using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Zinlo.Migrations
{
    public partial class closingchecklist : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_Comments_CommentId",
                table: "Tasks");

            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_AbpUsers_UserId1",
                table: "Tasks");

            migrationBuilder.DropIndex(
                name: "IX_Tasks_CommentId",
                table: "Tasks");

            migrationBuilder.DropIndex(
                name: "IX_Tasks_UserId1",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "CommentId",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "FilePath",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "Tasks");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ClosingMonth",
                table: "Tasks",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "CategoryId",
                table: "Tasks",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "Tasks",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn)
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AddColumn<long>(
                name: "AssigneeNameId",
                table: "Tasks",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<string>(
                name: "Attachment",
                table: "Tasks",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "DayBeforeAfter",
                table: "Tasks",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "DueOn",
                table: "Tasks",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "EndOfMonth",
                table: "Tasks",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "EndsOn",
                table: "Tasks",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "Frequency",
                table: "Tasks",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Instruction",
                table: "Tasks",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NoOfMonths",
                table: "Tasks",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<long>(
                name: "TypeId",
                table: "Comments",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "Type",
                table: "Comments",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext CHARACTER SET utf8mb4");

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "Categories",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn)
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_AssigneeNameId",
                table: "Tasks",
                column: "AssigneeNameId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_AbpUsers_AssigneeNameId",
                table: "Tasks",
                column: "AssigneeNameId",
                principalTable: "AbpUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_AbpUsers_AssigneeNameId",
                table: "Tasks");

            migrationBuilder.DropIndex(
                name: "IX_Tasks_AssigneeNameId",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "AssigneeNameId",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "Attachment",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "DayBeforeAfter",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "DueOn",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "EndOfMonth",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "EndsOn",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "Frequency",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "Instruction",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "NoOfMonths",
                table: "Tasks");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ClosingMonth",
                table: "Tasks",
                type: "datetime(6)",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<int>(
                name: "CategoryId",
                table: "Tasks",
                type: "int",
                nullable: false,
                oldClrType: typeof(long));

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Tasks",
                type: "int",
                nullable: false,
                oldClrType: typeof(long))
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn)
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AddColumn<int>(
                name: "CommentId",
                table: "Tasks",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "FilePath",
                table: "Tasks",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "Tasks",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<long>(
                name: "UserId1",
                table: "Tasks",
                type: "bigint",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "TypeId",
                table: "Comments",
                type: "int",
                nullable: false,
                oldClrType: typeof(long));

            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "Comments",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: false,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Categories",
                type: "int",
                nullable: false,
                oldClrType: typeof(long))
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn)
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_CommentId",
                table: "Tasks",
                column: "CommentId");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_UserId1",
                table: "Tasks",
                column: "UserId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_Comments_CommentId",
                table: "Tasks",
                column: "CommentId",
                principalTable: "Comments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_AbpUsers_UserId1",
                table: "Tasks",
                column: "UserId1",
                principalTable: "AbpUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
