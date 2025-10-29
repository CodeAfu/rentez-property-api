using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RentEZApi.Migrations
{
    /// <inheritdoc />
    public partial class PascalCaseUpdates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_docuseal_pdf_templates_AspNetUsers_OwnerId",
                table: "docuseal_pdf_templates");

            migrationBuilder.DropPrimaryKey(
                name: "PK_docuseal_pdf_templates",
                table: "docuseal_pdf_templates");

            migrationBuilder.RenameTable(
                name: "docuseal_pdf_templates",
                newName: "DocuSealPDFTemplates");

            migrationBuilder.RenameIndex(
                name: "IX_docuseal_pdf_templates_TemplateId",
                table: "DocuSealPDFTemplates",
                newName: "IX_DocuSealPDFTemplates_TemplateId");

            migrationBuilder.RenameIndex(
                name: "IX_docuseal_pdf_templates_OwnerId",
                table: "DocuSealPDFTemplates",
                newName: "IX_DocuSealPDFTemplates_OwnerId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DocuSealPDFTemplates",
                table: "DocuSealPDFTemplates",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_DocuSealPDFTemplates_AspNetUsers_OwnerId",
                table: "DocuSealPDFTemplates",
                column: "OwnerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DocuSealPDFTemplates_AspNetUsers_OwnerId",
                table: "DocuSealPDFTemplates");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DocuSealPDFTemplates",
                table: "DocuSealPDFTemplates");

            migrationBuilder.RenameTable(
                name: "DocuSealPDFTemplates",
                newName: "docuseal_pdf_templates");

            migrationBuilder.RenameIndex(
                name: "IX_DocuSealPDFTemplates_TemplateId",
                table: "docuseal_pdf_templates",
                newName: "IX_docuseal_pdf_templates_TemplateId");

            migrationBuilder.RenameIndex(
                name: "IX_DocuSealPDFTemplates_OwnerId",
                table: "docuseal_pdf_templates",
                newName: "IX_docuseal_pdf_templates_OwnerId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_docuseal_pdf_templates",
                table: "docuseal_pdf_templates",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_docuseal_pdf_templates_AspNetUsers_OwnerId",
                table: "docuseal_pdf_templates",
                column: "OwnerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
