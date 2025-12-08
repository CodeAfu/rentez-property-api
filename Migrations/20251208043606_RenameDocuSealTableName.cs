using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RentEZApi.Migrations
{
    /// <inheritdoc />
    public partial class RenameDocuSealTableName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DocuSealPDFTemplates_Users_OwnerId",
                table: "DocuSealPDFTemplates");

            migrationBuilder.DropForeignKey(
                name: "FK_PropertyListings_DocuSealPDFTemplates_AgreementId",
                table: "PropertyListings");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DocuSealPDFTemplates",
                table: "DocuSealPDFTemplates");

            migrationBuilder.RenameTable(
                name: "DocuSealPDFTemplates",
                newName: "DocuSealTemplates");

            migrationBuilder.RenameIndex(
                name: "IX_DocuSealPDFTemplates_TemplateId",
                table: "DocuSealTemplates",
                newName: "IX_DocuSealTemplates_TemplateId");

            migrationBuilder.RenameIndex(
                name: "IX_DocuSealPDFTemplates_OwnerId",
                table: "DocuSealTemplates",
                newName: "IX_DocuSealTemplates_OwnerId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DocuSealTemplates",
                table: "DocuSealTemplates",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_DocuSealTemplates_Users_OwnerId",
                table: "DocuSealTemplates",
                column: "OwnerId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PropertyListings_DocuSealTemplates_AgreementId",
                table: "PropertyListings",
                column: "AgreementId",
                principalTable: "DocuSealTemplates",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DocuSealTemplates_Users_OwnerId",
                table: "DocuSealTemplates");

            migrationBuilder.DropForeignKey(
                name: "FK_PropertyListings_DocuSealTemplates_AgreementId",
                table: "PropertyListings");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DocuSealTemplates",
                table: "DocuSealTemplates");

            migrationBuilder.RenameTable(
                name: "DocuSealTemplates",
                newName: "DocuSealPDFTemplates");

            migrationBuilder.RenameIndex(
                name: "IX_DocuSealTemplates_TemplateId",
                table: "DocuSealPDFTemplates",
                newName: "IX_DocuSealPDFTemplates_TemplateId");

            migrationBuilder.RenameIndex(
                name: "IX_DocuSealTemplates_OwnerId",
                table: "DocuSealPDFTemplates",
                newName: "IX_DocuSealPDFTemplates_OwnerId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DocuSealPDFTemplates",
                table: "DocuSealPDFTemplates",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_DocuSealPDFTemplates_Users_OwnerId",
                table: "DocuSealPDFTemplates",
                column: "OwnerId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PropertyListings_DocuSealPDFTemplates_AgreementId",
                table: "PropertyListings",
                column: "AgreementId",
                principalTable: "DocuSealPDFTemplates",
                principalColumn: "Id");
        }
    }
}
