using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RentEZApi.Migrations
{
    /// <inheritdoc />
    public partial class RemovePropertyIdFromTemplate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DocuSealTemplates_PropertyListings_PropertyId",
                table: "DocuSealTemplates");

            migrationBuilder.DropIndex(
                name: "IX_DocuSealTemplates_PropertyId",
                table: "DocuSealTemplates");

            migrationBuilder.DropColumn(
                name: "PropertyId",
                table: "DocuSealTemplates");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "PropertyId",
                table: "DocuSealTemplates",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_DocuSealTemplates_PropertyId",
                table: "DocuSealTemplates",
                column: "PropertyId");

            migrationBuilder.AddForeignKey(
                name: "FK_DocuSealTemplates_PropertyListings_PropertyId",
                table: "DocuSealTemplates",
                column: "PropertyId",
                principalTable: "PropertyListings",
                principalColumn: "Id");
        }
    }
}
