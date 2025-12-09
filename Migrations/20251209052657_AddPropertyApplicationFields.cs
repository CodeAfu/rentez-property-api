using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RentEZApi.Migrations
{
    /// <inheritdoc />
    public partial class AddPropertyApplicationFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "HasSentEmail",
                table: "PropertyApplications",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "HasSignedLease",
                table: "PropertyApplications",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsRenting",
                table: "PropertyApplications",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "RentAmount",
                table: "PropertyApplications",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "Slug",
                table: "DocuSealLeaseSubmissions",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HasSentEmail",
                table: "PropertyApplications");

            migrationBuilder.DropColumn(
                name: "HasSignedLease",
                table: "PropertyApplications");

            migrationBuilder.DropColumn(
                name: "IsRenting",
                table: "PropertyApplications");

            migrationBuilder.DropColumn(
                name: "RentAmount",
                table: "PropertyApplications");

            migrationBuilder.DropColumn(
                name: "Slug",
                table: "DocuSealLeaseSubmissions");
        }
    }
}
