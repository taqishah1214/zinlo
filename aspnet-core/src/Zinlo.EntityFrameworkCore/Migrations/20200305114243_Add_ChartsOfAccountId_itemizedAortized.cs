using Microsoft.EntityFrameworkCore.Migrations;

namespace Zinlo.Migrations
{
    public partial class Add_ChartsOfAccountId_itemizedAortized : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "ChartsofAccountId",
                table: "Itemizations",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "ChartsofAccountId",
                table: "Amortizations",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_Itemizations_ChartsofAccountId",
                table: "Itemizations",
                column: "ChartsofAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Amortizations_ChartsofAccountId",
                table: "Amortizations",
                column: "ChartsofAccountId");

            migrationBuilder.AddForeignKey(
                name: "FK_Amortizations_ChartsofAccount_ChartsofAccountId",
                table: "Amortizations",
                column: "ChartsofAccountId",
                principalTable: "ChartsofAccount",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Itemizations_ChartsofAccount_ChartsofAccountId",
                table: "Itemizations",
                column: "ChartsofAccountId",
                principalTable: "ChartsofAccount",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Amortizations_ChartsofAccount_ChartsofAccountId",
                table: "Amortizations");

            migrationBuilder.DropForeignKey(
                name: "FK_Itemizations_ChartsofAccount_ChartsofAccountId",
                table: "Itemizations");

            migrationBuilder.DropIndex(
                name: "IX_Itemizations_ChartsofAccountId",
                table: "Itemizations");

            migrationBuilder.DropIndex(
                name: "IX_Amortizations_ChartsofAccountId",
                table: "Amortizations");

            migrationBuilder.DropColumn(
                name: "ChartsofAccountId",
                table: "Itemizations");

            migrationBuilder.DropColumn(
                name: "ChartsofAccountId",
                table: "Amortizations");
        }
    }
}
