using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RentEZApi.Migrations
{
    /// <inheritdoc />
    public partial class LotsOfChangesIdk : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DocumentJson",
                table: "DocuSealPDFTemplates");

            migrationBuilder.AlterColumn<string>(
                name: "SubmittersJson",
                table: "DocuSealPDFTemplates",
                type: "jsonb",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "jsonb");

            migrationBuilder.AlterColumn<string>(
                name: "Slug",
                table: "DocuSealPDFTemplates",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(255)",
                oldMaxLength: 255,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DocumentsJson",
                table: "DocuSealPDFTemplates",
                type: "jsonb",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FieldsJson",
                table: "DocuSealPDFTemplates",
                type: "jsonb",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DocumentsJson",
                table: "DocuSealPDFTemplates");

            migrationBuilder.DropColumn(
                name: "FieldsJson",
                table: "DocuSealPDFTemplates");

            migrationBuilder.AlterColumn<string>(
                name: "SubmittersJson",
                table: "DocuSealPDFTemplates",
                type: "jsonb",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "jsonb",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Slug",
                table: "DocuSealPDFTemplates",
                type: "character varying(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(255)",
                oldMaxLength: 255);

            migrationBuilder.AddColumn<string>(
                name: "DocumentJson",
                table: "DocuSealPDFTemplates",
                type: "jsonb",
                nullable: false,
                defaultValue: "");
        }
    }
}
