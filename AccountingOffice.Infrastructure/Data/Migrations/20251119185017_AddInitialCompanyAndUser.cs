using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AccountingOffice.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddInitialCompanyAndUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InstallmentsPayable_AccountsPayable_AccountId",
                schema: "dbo",
                table: "InstallmentsPayable");

            migrationBuilder.DropForeignKey(
                name: "FK_InstallmentsReceivable_AccountsReceivable_AccountId",
                schema: "dbo",
                table: "InstallmentsReceivable");

            migrationBuilder.InsertData(
                schema: "dbo",
                table: "Companies",
                columns: new[] { "Id", "Active", "CreatedAt", "Document", "Email", "Name", "Phone" },
                values: new object[] { new Guid("11111111-1111-1111-1111-111111111111"), true, new DateTime(2025, 11, 19, 18, 50, 16, 324, DateTimeKind.Utc).AddTicks(4552), "48.245.009/0001-99", "cia@microworkes.com.br", "Microworkers do Brasil", "(27)90004-5444" });

            migrationBuilder.InsertData(
                schema: "dbo",
                table: "Users",
                columns: new[] { "Id", "Active", "CreatedAt", "Password", "TenantId", "UserName" },
                values: new object[] { 1, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Abcd1234****", new Guid("11111111-1111-1111-1111-111111111111"), "Alexandre" });

            migrationBuilder.AddForeignKey(
                name: "FK_InstallmentsPayable_AccountsPayable_AccountId",
                schema: "dbo",
                table: "InstallmentsPayable",
                column: "AccountId",
                principalSchema: "dbo",
                principalTable: "AccountsPayable",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_InstallmentsReceivable_AccountsReceivable_AccountId",
                schema: "dbo",
                table: "InstallmentsReceivable",
                column: "AccountId",
                principalSchema: "dbo",
                principalTable: "AccountsReceivable",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InstallmentsPayable_AccountsPayable_AccountId",
                schema: "dbo",
                table: "InstallmentsPayable");

            migrationBuilder.DropForeignKey(
                name: "FK_InstallmentsReceivable_AccountsReceivable_AccountId",
                schema: "dbo",
                table: "InstallmentsReceivable");

            migrationBuilder.DeleteData(
                schema: "dbo",
                table: "Companies",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"));

            migrationBuilder.DeleteData(
                schema: "dbo",
                table: "Users",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.AddForeignKey(
                name: "FK_InstallmentsPayable_AccountsPayable_AccountId",
                schema: "dbo",
                table: "InstallmentsPayable",
                column: "AccountId",
                principalSchema: "dbo",
                principalTable: "AccountsPayable",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_InstallmentsReceivable_AccountsReceivable_AccountId",
                schema: "dbo",
                table: "InstallmentsReceivable",
                column: "AccountId",
                principalSchema: "dbo",
                principalTable: "AccountsReceivable",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
