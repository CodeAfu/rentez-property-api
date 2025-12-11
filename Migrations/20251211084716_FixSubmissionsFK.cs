using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RentEZApi.Migrations
{
    /// <inheritdoc />
    public partial class FixSubmissionsFK : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "PropertyApplicationId",
                table: "DocuSealSubmissions",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_DocuSealSubmissions_PropertyApplicationId",
                table: "DocuSealSubmissions",
                column: "PropertyApplicationId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_DocuSealSubmissions_PropertyApplications_PropertyApplicatio~",
                table: "DocuSealSubmissions",
                column: "PropertyApplicationId",
                principalTable: "PropertyApplications",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DocuSealSubmissions_PropertyApplications_PropertyApplicatio~",
                table: "DocuSealSubmissions");

            migrationBuilder.DropIndex(
                name: "IX_DocuSealSubmissions_PropertyApplicationId",
                table: "DocuSealSubmissions");

            migrationBuilder.DropColumn(
                name: "PropertyApplicationId",
                table: "DocuSealSubmissions");
        }
    }
}
