using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RentEZApi.Migrations
{
    /// <inheritdoc />
    public partial class RefinePropertyRelationships : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PropertyListings_DocuSealTemplates_AgreementId",
                table: "PropertyListings");

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

            migrationBuilder.AddForeignKey(
                name: "FK_PropertyListings_DocuSealTemplates_AgreementId",
                table: "PropertyListings",
                column: "AgreementId",
                principalTable: "DocuSealTemplates",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DocuSealTemplates_PropertyListings_PropertyId",
                table: "DocuSealTemplates");

            migrationBuilder.DropForeignKey(
                name: "FK_PropertyListings_DocuSealTemplates_AgreementId",
                table: "PropertyListings");

            migrationBuilder.DropIndex(
                name: "IX_DocuSealTemplates_PropertyId",
                table: "DocuSealTemplates");

            migrationBuilder.DropColumn(
                name: "PropertyId",
                table: "DocuSealTemplates");

            migrationBuilder.AddForeignKey(
                name: "FK_PropertyListings_DocuSealTemplates_AgreementId",
                table: "PropertyListings",
                column: "AgreementId",
                principalTable: "DocuSealTemplates",
                principalColumn: "Id");
        }
    }
}
