using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RentEZApi.Migrations
{
    /// <inheritdoc />
    public partial class EntityRelationshipAdjustments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "AgreementId",
                table: "Property",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "OwnerId",
                table: "Property",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<decimal>(
                name: "Rent",
                table: "Property",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateIndex(
                name: "IX_Property_AgreementId",
                table: "Property",
                column: "AgreementId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Property_OwnerId",
                table: "Property",
                column: "OwnerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Property_DocuSealPDFTemplates_AgreementId",
                table: "Property",
                column: "AgreementId",
                principalTable: "DocuSealPDFTemplates",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Property_Users_OwnerId",
                table: "Property",
                column: "OwnerId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Property_DocuSealPDFTemplates_AgreementId",
                table: "Property");

            migrationBuilder.DropForeignKey(
                name: "FK_Property_Users_OwnerId",
                table: "Property");

            migrationBuilder.DropIndex(
                name: "IX_Property_AgreementId",
                table: "Property");

            migrationBuilder.DropIndex(
                name: "IX_Property_OwnerId",
                table: "Property");

            migrationBuilder.DropColumn(
                name: "AgreementId",
                table: "Property");

            migrationBuilder.DropColumn(
                name: "OwnerId",
                table: "Property");

            migrationBuilder.DropColumn(
                name: "Rent",
                table: "Property");
        }
    }
}
