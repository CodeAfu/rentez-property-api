using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RentEZApi.Migrations
{
    /// <inheritdoc />
    public partial class UpdatePropertyApplicationsColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PropertyApplications_ApplicantProfiles_ApplicantId",
                table: "PropertyApplications");

            migrationBuilder.RenameColumn(
                name: "ApplicantId",
                table: "PropertyApplications",
                newName: "ApplicantProfileId");

            migrationBuilder.RenameIndex(
                name: "IX_PropertyApplications_ApplicantId",
                table: "PropertyApplications",
                newName: "IX_PropertyApplications_ApplicantProfileId");

            migrationBuilder.AddForeignKey(
                name: "FK_PropertyApplications_ApplicantProfiles_ApplicantProfileId",
                table: "PropertyApplications",
                column: "ApplicantProfileId",
                principalTable: "ApplicantProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PropertyApplications_ApplicantProfiles_ApplicantProfileId",
                table: "PropertyApplications");

            migrationBuilder.RenameColumn(
                name: "ApplicantProfileId",
                table: "PropertyApplications",
                newName: "ApplicantId");

            migrationBuilder.RenameIndex(
                name: "IX_PropertyApplications_ApplicantProfileId",
                table: "PropertyApplications",
                newName: "IX_PropertyApplications_ApplicantId");

            migrationBuilder.AddForeignKey(
                name: "FK_PropertyApplications_ApplicantProfiles_ApplicantId",
                table: "PropertyApplications",
                column: "ApplicantId",
                principalTable: "ApplicantProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
