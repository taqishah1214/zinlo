using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Zinlo.Migrations
{
    public partial class ChartsofAccount_Table_Added : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ChartsofAccount",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: false),
                    AccountNo = table.Column<long>(nullable: false),
                    AccountName = table.Column<string>(nullable: true),
                    AccountType = table.Column<int>(nullable: false),
                    ReconciliationType = table.Column<int>(nullable: false),
                    AccountSubTypeId = table.Column<long>(nullable: false),
                    AssigneeId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChartsofAccount", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChartsofAccount_AccountSubTypes_AccountSubTypeId",
                        column: x => x.AccountSubTypeId,
                        principalTable: "AccountSubTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChartsofAccount_AbpUsers_AssigneeId",
                        column: x => x.AssigneeId,
                        principalTable: "AbpUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChartsofAccount_AccountSubTypeId",
                table: "ChartsofAccount",
                column: "AccountSubTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ChartsofAccount_AssigneeId",
                table: "ChartsofAccount",
                column: "AssigneeId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChartsofAccount");
        }
    }
}
