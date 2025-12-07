using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RentEZApi.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSubmissionsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DocuSealLeaseSubmissions_Property_PropertyId",
                table: "DocuSealLeaseSubmissions");

            migrationBuilder.DropForeignKey(
                name: "FK_Property_DocuSealPDFTemplates_AgreementId",
                table: "Property");

            migrationBuilder.DropForeignKey(
                name: "FK_Property_Users_OwnerId",
                table: "Property");

            migrationBuilder.DropForeignKey(
                name: "FK_PropertyApplications_Property_PropertyId",
                table: "PropertyApplications");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Property",
                table: "Property");

            migrationBuilder.RenameTable(
                name: "Property",
                newName: "PropertyListings");

            migrationBuilder.RenameIndex(
                name: "IX_Property_OwnerId_Hash",
                table: "PropertyListings",
                newName: "IX_PropertyListings_OwnerId_Hash");

            migrationBuilder.RenameIndex(
                name: "IX_Property_OwnerId",
                table: "PropertyListings",
                newName: "IX_PropertyListings_OwnerId");

            migrationBuilder.RenameIndex(
                name: "IX_Property_AgreementId",
                table: "PropertyListings",
                newName: "IX_PropertyListings_AgreementId");

            migrationBuilder.Sql(@"
                ALTER TABLE ""DocuSealLeaseSubmissions"" 
                ALTER COLUMN ""OpenedAt"" TYPE timestamp with time zone 
                USING NULLIF(""OpenedAt"", '')::timestamp with time zone;
            ");

            migrationBuilder.Sql(@"
                ALTER TABLE ""DocuSealLeaseSubmissions"" 
                ALTER COLUMN ""DeclinedAt"" TYPE timestamp with time zone 
                USING NULLIF(""DeclinedAt"", '')::timestamp with time zone;
            ");

            migrationBuilder.Sql(@"
                ALTER TABLE ""DocuSealLeaseSubmissions"" 
                ALTER COLUMN ""CompletedAt"" TYPE timestamp with time zone 
                USING NULLIF(""CompletedAt"", '')::timestamp with time zone;
            ");

            migrationBuilder.AddColumn<string>(
                name: "FolderName",
                table: "DocuSealLeaseSubmissions",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "SubmissionId",
                table: "DocuSealLeaseSubmissions",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddPrimaryKey(
                name: "PK_PropertyListings",
                table: "PropertyListings",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_DocuSealLeaseSubmissions_SubmissionId",
                table: "DocuSealLeaseSubmissions",
                column: "SubmissionId");

            migrationBuilder.AddForeignKey(
                name: "FK_DocuSealLeaseSubmissions_PropertyListings_PropertyId",
                table: "DocuSealLeaseSubmissions",
                column: "PropertyId",
                principalTable: "PropertyListings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PropertyApplications_PropertyListings_PropertyId",
                table: "PropertyApplications",
                column: "PropertyId",
                principalTable: "PropertyListings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PropertyListings_DocuSealPDFTemplates_AgreementId",
                table: "PropertyListings",
                column: "AgreementId",
                principalTable: "DocuSealPDFTemplates",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PropertyListings_Users_OwnerId",
                table: "PropertyListings",
                column: "OwnerId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DocuSealLeaseSubmissions_PropertyListings_PropertyId",
                table: "DocuSealLeaseSubmissions");

            migrationBuilder.DropForeignKey(
                name: "FK_PropertyApplications_PropertyListings_PropertyId",
                table: "PropertyApplications");

            migrationBuilder.DropForeignKey(
                name: "FK_PropertyListings_DocuSealPDFTemplates_AgreementId",
                table: "PropertyListings");

            migrationBuilder.DropForeignKey(
                name: "FK_PropertyListings_Users_OwnerId",
                table: "PropertyListings");

            migrationBuilder.DropIndex(
                name: "IX_DocuSealLeaseSubmissions_SubmissionId",
                table: "DocuSealLeaseSubmissions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PropertyListings",
                table: "PropertyListings");

            migrationBuilder.DropColumn(
                name: "FolderName",
                table: "DocuSealLeaseSubmissions");

            migrationBuilder.DropColumn(
                name: "SubmissionId",
                table: "DocuSealLeaseSubmissions");

            migrationBuilder.RenameTable(
                name: "PropertyListings",
                newName: "Property");

            migrationBuilder.RenameIndex(
                name: "IX_PropertyListings_OwnerId_Hash",
                table: "Property",
                newName: "IX_Property_OwnerId_Hash");

            migrationBuilder.RenameIndex(
                name: "IX_PropertyListings_OwnerId",
                table: "Property",
                newName: "IX_Property_OwnerId");

            migrationBuilder.RenameIndex(
                name: "IX_PropertyListings_AgreementId",
                table: "Property",
                newName: "IX_Property_AgreementId");

            migrationBuilder.AlterColumn<string>(
                name: "OpenedAt",
                table: "DocuSealLeaseSubmissions",
                type: "text",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DeclinedAt",
                table: "DocuSealLeaseSubmissions",
                type: "text",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CompletedAt",
                table: "DocuSealLeaseSubmissions",
                type: "text",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Property",
                table: "Property",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_DocuSealLeaseSubmissions_Property_PropertyId",
                table: "DocuSealLeaseSubmissions",
                column: "PropertyId",
                principalTable: "Property",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

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

            migrationBuilder.AddForeignKey(
                name: "FK_PropertyApplications_Property_PropertyId",
                table: "PropertyApplications",
                column: "PropertyId",
                principalTable: "Property",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
