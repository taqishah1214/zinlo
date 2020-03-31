using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Zinlo.Migrations
{
    public partial class ChartOfAccounts_Correction : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Amortizations_ChartsofAccount_ChartsofAccountId",
                table: "Amortizations");

            migrationBuilder.DropForeignKey(
                name: "FK_Itemizations_ChartsofAccount_ChartsofAccountId",
                table: "Itemizations");

            migrationBuilder.DropTable(
                name: "ChartsofAccount");

            migrationBuilder.CreateTable(
                name: "ChartofAccounts",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    DeleterUserId = table.Column<long>(nullable: true),
                    DeletionTime = table.Column<DateTime>(nullable: true),
                    TenantId = table.Column<int>(nullable: false),
                    AccountNumber = table.Column<string>(nullable: true),
                    AccountName = table.Column<string>(nullable: true),
                    AccountType = table.Column<int>(nullable: false),
                    ReconciliationType = table.Column<int>(nullable: false),
                    AssigneeId = table.Column<long>(nullable: false),
                    AccountSubTypeId = table.Column<long>(nullable: false),
                    Status = table.Column<int>(nullable: false),
                    Reconciled = table.Column<int>(nullable: false),
                    Balance = table.Column<double>(nullable: false),
                    Lock = table.Column<bool>(nullable: false),
                    TrialBalance = table.Column<decimal>(nullable: false),
                    VersionId = table.Column<long>(nullable: false),
                    ClosingMonth = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChartofAccounts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChartofAccounts_AccountSubTypes_AccountSubTypeId",
                        column: x => x.AccountSubTypeId,
                        principalTable: "AccountSubTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChartofAccounts_AbpUsers_AssigneeId",
                        column: x => x.AssigneeId,
                        principalTable: "AbpUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChartofAccounts_AccountSubTypeId",
                table: "ChartofAccounts",
                column: "AccountSubTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ChartofAccounts_AssigneeId",
                table: "ChartofAccounts",
                column: "AssigneeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Amortizations_ChartofAccounts_ChartsofAccountId",
                table: "Amortizations",
                column: "ChartsofAccountId",
                principalTable: "ChartofAccounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Itemizations_ChartofAccounts_ChartsofAccountId",
                table: "Itemizations",
                column: "ChartsofAccountId",
                principalTable: "ChartofAccounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Amortizations_ChartofAccounts_ChartsofAccountId",
                table: "Amortizations");

            migrationBuilder.DropForeignKey(
                name: "FK_Itemizations_ChartofAccounts_ChartsofAccountId",
                table: "Itemizations");

            migrationBuilder.DropTable(
                name: "ChartofAccounts");

            migrationBuilder.CreateTable(
                name: "ChartsofAccount",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AccountName = table.Column<string>(type: "text", nullable: true),
                    AccountNumber = table.Column<string>(type: "text", nullable: true),
                    AccountSubTypeId = table.Column<long>(type: "bigint", nullable: false),
                    AccountType = table.Column<int>(type: "integer", nullable: false),
                    AssigneeId = table.Column<long>(type: "bigint", nullable: false),
                    Balance = table.Column<double>(type: "double precision", nullable: false),
                    ClosingMonth = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: true),
                    IsArchive = table.Column<bool>(type: "boolean", nullable: false),
                    Lock = table.Column<bool>(type: "boolean", nullable: false),
                    Reconciled = table.Column<int>(type: "integer", nullable: false),
                    ReconciliationType = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    TenantId = table.Column<int>(type: "integer", nullable: false),
                    TrialBalance = table.Column<decimal>(type: "numeric", nullable: false),
                    VersionId = table.Column<long>(type: "bigint", nullable: false)
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
    }
}
