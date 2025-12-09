using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RentEZApi.Migrations
{
    /// <inheritdoc />
    public partial class RefineSubmissionsAndFKs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DocuSealSubmissions_PropertyListings_PropertyId",
                table: "DocuSealSubmissions");

            migrationBuilder.RenameColumn(
                name: "Slug",
                table: "DocuSealSubmissions",
                newName: "SignerSlug");

            migrationBuilder.AddColumn<Guid>(
                name: "SignerId",
                table: "DocuSealSubmissions",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_DocuSealSubmissions_SignerId",
                table: "DocuSealSubmissions",
                column: "SignerId");

            migrationBuilder.AddForeignKey(
                name: "FK_DocuSealSubmissions_PropertyListings_PropertyId",
                table: "DocuSealSubmissions",
                column: "PropertyId",
                principalTable: "PropertyListings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DocuSealSubmissions_Users_SignerId",
                table: "DocuSealSubmissions",
                column: "SignerId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DocuSealSubmissions_PropertyListings_PropertyId",
                table: "DocuSealSubmissions");

            migrationBuilder.DropForeignKey(
                name: "FK_DocuSealSubmissions_Users_SignerId",
                table: "DocuSealSubmissions");

            migrationBuilder.DropIndex(
                name: "IX_DocuSealSubmissions_SignerId",
                table: "DocuSealSubmissions");

            migrationBuilder.DropColumn(
                name: "SignerId",
                table: "DocuSealSubmissions");

            migrationBuilder.RenameColumn(
                name: "SignerSlug",
                table: "DocuSealSubmissions",
                newName: "Slug");

            migrationBuilder.AddForeignKey(
                name: "FK_DocuSealSubmissions_PropertyListings_PropertyId",
                table: "DocuSealSubmissions",
                column: "PropertyId",
                principalTable: "PropertyListings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
