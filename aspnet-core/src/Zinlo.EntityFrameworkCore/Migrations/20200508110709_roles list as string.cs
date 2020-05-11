using Microsoft.EntityFrameworkCore.Migrations;

namespace Zinlo.Migrations
{
    public partial class roleslistasstring : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "RoleId",
                table: "InviteUsers",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserName",
                table: "InviteUsers",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserName",
                table: "InviteUsers");

            migrationBuilder.AlterColumn<int>(
                name: "RoleId",
                table: "InviteUsers",
                type: "integer",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);
        }
    }
}
