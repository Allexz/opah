using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AccountingOffice.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "dbo");

            migrationBuilder.CreateTable(
                name: "Companies",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false, collation: "SQL_Latin1_General_CP1_CI_AS"),
                    Document = table.Column<string>(type: "nvarchar(18)", maxLength: 18, nullable: false, collation: "SQL_Latin1_General_CP1_CI_AS"),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false, collation: "SQL_Latin1_General_CP1_CI_AS"),
                    Phone = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false, collation: "SQL_Latin1_General_CP1_CI_AS"),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Companies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Persons",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false, collation: "SQL_Latin1_General_CP1_CI_AS"),
                    Document = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, collation: "SQL_Latin1_General_CP1_CI_AS"),
                    Type = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false, collation: "SQL_Latin1_General_CP1_CI_AS"),
                    Phone = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false, collation: "SQL_Latin1_General_CP1_CI_AS"),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    PersonType = table.Column<int>(type: "int", nullable: false),
                    MaritalStatus = table.Column<int>(type: "int", nullable: true),
                    LegalName = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true, collation: "SQL_Latin1_General_CP1_CI_AS")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Persons", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false, collation: "SQL_Latin1_General_CP1_CI_AS"),
                    Password = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false, collation: "SQL_Latin1_General_CP1_CI_AS"),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AccountsPayable",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PaymentDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false, collation: "SQL_Latin1_General_CP1_CI_AS"),
                    Ammount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IssueDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DueDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    RelatedPartyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PayMethod = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountsPayable", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AccountsPayable_Persons_RelatedPartyId",
                        column: x => x.RelatedPartyId,
                        principalSchema: "dbo",
                        principalTable: "Persons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AccountsReceivable",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InvoiceNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, collation: "SQL_Latin1_General_CP1_CI_AS"),
                    ReceivedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false, collation: "SQL_Latin1_General_CP1_CI_AS"),
                    Ammount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IssueDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DueDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    RelatedPartyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PayMethod = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountsReceivable", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AccountsReceivable_Persons_RelatedPartyId",
                        column: x => x.RelatedPartyId,
                        principalSchema: "dbo",
                        principalTable: "Persons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "InstallmentsPayable",
                schema: "dbo",
                columns: table => new
                {
                    InstallmentNumber = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DueDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    PaymentDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Entrytype = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InstallmentsPayable", x => new { x.AccountId, x.InstallmentNumber });
                    table.ForeignKey(
                        name: "FK_InstallmentsPayable_AccountsPayable_AccountId",
                        column: x => x.AccountId,
                        principalSchema: "dbo",
                        principalTable: "AccountsPayable",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "InstallmentsReceivable",
                schema: "dbo",
                columns: table => new
                {
                    InstallmentNumber = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DueDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    PaymentDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Entrytype = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InstallmentsReceivable", x => new { x.AccountId, x.InstallmentNumber });
                    table.ForeignKey(
                        name: "FK_InstallmentsReceivable_AccountsReceivable_AccountId",
                        column: x => x.AccountId,
                        principalSchema: "dbo",
                        principalTable: "AccountsReceivable",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AccountsPayable_RelatedPartyId",
                schema: "dbo",
                table: "AccountsPayable",
                column: "RelatedPartyId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountsPayable_TenantId",
                schema: "dbo",
                table: "AccountsPayable",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountsReceivable_RelatedPartyId",
                schema: "dbo",
                table: "AccountsReceivable",
                column: "RelatedPartyId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountsReceivable_TenantId",
                schema: "dbo",
                table: "AccountsReceivable",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountsReceivable_TenantId_InvoiceNumber",
                schema: "dbo",
                table: "AccountsReceivable",
                columns: new[] { "TenantId", "InvoiceNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Companies_Document",
                schema: "dbo",
                table: "Companies",
                column: "Document",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Persons_TenantId_Document",
                schema: "dbo",
                table: "Persons",
                columns: new[] { "TenantId", "Document" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_TenantId_UserName",
                schema: "dbo",
                table: "Users",
                columns: new[] { "TenantId", "UserName" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Companies",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "InstallmentsPayable",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "InstallmentsReceivable",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Users",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "AccountsPayable",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "AccountsReceivable",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Persons",
                schema: "dbo");
        }
    }
}
