using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RentEZApi.Migrations
{
    /// <inheritdoc />
    public partial class AddRealSubmissionDocumentTemp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "DocumentData",
                table: "DocuSealLeaseSubmissions",
                type: "bytea",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DocumentFileName",
                table: "DocuSealLeaseSubmissions",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DocumentData",
                table: "DocuSealLeaseSubmissions");

            migrationBuilder.DropColumn(
                name: "DocumentFileName",
                table: "DocuSealLeaseSubmissions");
        }
    }
}
