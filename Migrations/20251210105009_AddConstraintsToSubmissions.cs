using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RentEZApi.Migrations
{
    /// <inheritdoc />
    public partial class AddConstraintsToSubmissions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_DocuSealSubmissions_SignerId",
                table: "DocuSealSubmissions");

            migrationBuilder.CreateIndex(
                name: "IX_DocuSealSubmissions_SignerId_PropertyId",
                table: "DocuSealSubmissions",
                columns: new[] { "SignerId", "PropertyId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DocuSealSubmissions_SignerSlug",
                table: "DocuSealSubmissions",
                column: "SignerSlug");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_DocuSealSubmissions_SignerId_PropertyId",
                table: "DocuSealSubmissions");

            migrationBuilder.DropIndex(
                name: "IX_DocuSealSubmissions_SignerSlug",
                table: "DocuSealSubmissions");

            migrationBuilder.CreateIndex(
                name: "IX_DocuSealSubmissions_SignerId",
                table: "DocuSealSubmissions",
                column: "SignerId");
        }
    }
}
