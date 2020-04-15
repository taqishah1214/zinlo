using Microsoft.EntityFrameworkCore.Migrations;

namespace Zinlo.Migrations
{
    public partial class removeuserformpaymettable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserPaymentDetails_AbpUsers_UserId",
                table: "UserPaymentDetails");

            migrationBuilder.DropIndex(
                name: "IX_UserPaymentDetails_UserId",
                table: "UserPaymentDetails");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "UserPaymentDetails");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "UserId",
                table: "UserPaymentDetails",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserPaymentDetails_UserId",
                table: "UserPaymentDetails",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserPaymentDetails_AbpUsers_UserId",
                table: "UserPaymentDetails",
                column: "UserId",
                principalTable: "AbpUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
