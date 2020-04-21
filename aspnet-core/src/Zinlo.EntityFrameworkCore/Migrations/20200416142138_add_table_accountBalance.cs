using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Zinlo.Migrations
{
    public partial class add_table_accountBalance : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Balance",
                table: "ChartofAccounts");

            migrationBuilder.DropColumn(
                name: "ClosingMonth",
                table: "ChartofAccounts");

            migrationBuilder.DropColumn(
                name: "Lock",
                table: "ChartofAccounts");

            migrationBuilder.DropColumn(
                name: "TrialBalance",
                table: "ChartofAccounts");

            migrationBuilder.CreateTable(
                name: "AccountBalance",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    AccountId = table.Column<long>(nullable: false),
                    Balance = table.Column<double>(nullable: false),
                    TrialBalance = table.Column<double>(nullable: false),
                    Month = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountBalance", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AccountBalance_ChartofAccounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "ChartofAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AccountBalance_AccountId",
                table: "AccountBalance",
                column: "AccountId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccountBalance");

            migrationBuilder.AddColumn<double>(
                name: "Balance",
                table: "ChartofAccounts",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<DateTime>(
                name: "ClosingMonth",
                table: "ChartofAccounts",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "Lock",
                table: "ChartofAccounts",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "TrialBalance",
                table: "ChartofAccounts",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
