using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RentEZApi.Migrations
{
    /// <inheritdoc />
    public partial class UpdateApiIdColumnName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DocuSealLeaseSubmissions");

            migrationBuilder.RenameColumn(
                name: "TemplateId",
                table: "DocuSealTemplates",
                newName: "APITemplateId");

            migrationBuilder.RenameIndex(
                name: "IX_DocuSealTemplates_TemplateId",
                table: "DocuSealTemplates",
                newName: "IX_DocuSealTemplates_APITemplateId");

            migrationBuilder.CreateTable(
                name: "DocuSealSubmissions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    APISubmissionId = table.Column<long>(type: "bigint", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Email = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<string>(type: "text", nullable: true),
                    Role = table.Column<string>(type: "text", nullable: true),
                    Slug = table.Column<string>(type: "text", nullable: true),
                    DocumentData = table.Column<byte[]>(type: "bytea", nullable: true),
                    DocumentFileName = table.Column<string>(type: "text", nullable: true),
                    PropertyId = table.Column<Guid>(type: "uuid", nullable: false),
                    OpenedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CompletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeclinedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocuSealSubmissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DocuSealSubmissions_PropertyListings_PropertyId",
                        column: x => x.PropertyId,
                        principalTable: "PropertyListings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DocuSealSubmissions_APISubmissionId",
                table: "DocuSealSubmissions",
                column: "APISubmissionId");

            migrationBuilder.CreateIndex(
                name: "IX_DocuSealSubmissions_Email",
                table: "DocuSealSubmissions",
                column: "Email");

            migrationBuilder.CreateIndex(
                name: "IX_DocuSealSubmissions_PropertyId",
                table: "DocuSealSubmissions",
                column: "PropertyId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DocuSealSubmissions");

            migrationBuilder.RenameColumn(
                name: "APITemplateId",
                table: "DocuSealTemplates",
                newName: "TemplateId");

            migrationBuilder.RenameIndex(
                name: "IX_DocuSealTemplates_APITemplateId",
                table: "DocuSealTemplates",
                newName: "IX_DocuSealTemplates_TemplateId");

            migrationBuilder.CreateTable(
                name: "DocuSealLeaseSubmissions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PropertyId = table.Column<Guid>(type: "uuid", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    DeclinedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DocumentData = table.Column<byte[]>(type: "bytea", nullable: true),
                    DocumentFileName = table.Column<string>(type: "text", nullable: true),
                    Email = table.Column<string>(type: "text", nullable: true),
                    ExternalId = table.Column<string>(type: "text", nullable: true),
                    Name = table.Column<string>(type: "text", nullable: true),
                    OpenedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Role = table.Column<string>(type: "text", nullable: true),
                    Slug = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<string>(type: "text", nullable: true),
                    SubmissionId = table.Column<long>(type: "bigint", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocuSealLeaseSubmissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DocuSealLeaseSubmissions_PropertyListings_PropertyId",
                        column: x => x.PropertyId,
                        principalTable: "PropertyListings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DocuSealLeaseSubmissions_Email",
                table: "DocuSealLeaseSubmissions",
                column: "Email");

            migrationBuilder.CreateIndex(
                name: "IX_DocuSealLeaseSubmissions_ExternalId",
                table: "DocuSealLeaseSubmissions",
                column: "ExternalId");

            migrationBuilder.CreateIndex(
                name: "IX_DocuSealLeaseSubmissions_PropertyId",
                table: "DocuSealLeaseSubmissions",
                column: "PropertyId");

            migrationBuilder.CreateIndex(
                name: "IX_DocuSealLeaseSubmissions_SubmissionId",
                table: "DocuSealLeaseSubmissions",
                column: "SubmissionId");
        }
    }
}
