using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RentEZApi.Migrations
{
    /// <inheritdoc />
    public partial class AddLeaseSubmissions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DocuSealLeaseSubmissions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Email = table.Column<string>(type: "text", nullable: true),
                    ExternalId = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<string>(type: "text", nullable: true),
                    Role = table.Column<string>(type: "text", nullable: true),
                    OpenedAt = table.Column<string>(type: "text", nullable: true),
                    CompletedAt = table.Column<string>(type: "text", nullable: true),
                    DeclinedAt = table.Column<string>(type: "text", nullable: true),
                    PropertyId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocuSealLeaseSubmissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DocuSealLeaseSubmissions_Property_PropertyId",
                        column: x => x.PropertyId,
                        principalTable: "Property",
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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DocuSealLeaseSubmissions");
        }
    }
}
