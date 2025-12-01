using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RentEZApi.Migrations
{
    /// <inheritdoc />
    public partial class AlterTableNameApplicantProfiles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ApplicantProfile_Users_UserId",
                table: "ApplicantProfile");

            migrationBuilder.DropForeignKey(
                name: "FK_PropertyApplications_ApplicantProfile_ApplicantId",
                table: "PropertyApplications");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ApplicantProfile",
                table: "ApplicantProfile");

            migrationBuilder.RenameTable(
                name: "ApplicantProfile",
                newName: "ApplicantProfiles");

            migrationBuilder.RenameIndex(
                name: "IX_ApplicantProfile_UserId",
                table: "ApplicantProfiles",
                newName: "IX_ApplicantProfiles_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_ApplicantProfile_GovernmentIdNumber",
                table: "ApplicantProfiles",
                newName: "IX_ApplicantProfiles_GovernmentIdNumber");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ApplicantProfiles",
                table: "ApplicantProfiles",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ApplicantProfiles_Users_UserId",
                table: "ApplicantProfiles",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PropertyApplications_ApplicantProfiles_ApplicantId",
                table: "PropertyApplications",
                column: "ApplicantId",
                principalTable: "ApplicantProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ApplicantProfiles_Users_UserId",
                table: "ApplicantProfiles");

            migrationBuilder.DropForeignKey(
                name: "FK_PropertyApplications_ApplicantProfiles_ApplicantId",
                table: "PropertyApplications");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ApplicantProfiles",
                table: "ApplicantProfiles");

            migrationBuilder.RenameTable(
                name: "ApplicantProfiles",
                newName: "ApplicantProfile");

            migrationBuilder.RenameIndex(
                name: "IX_ApplicantProfiles_UserId",
                table: "ApplicantProfile",
                newName: "IX_ApplicantProfile_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_ApplicantProfiles_GovernmentIdNumber",
                table: "ApplicantProfile",
                newName: "IX_ApplicantProfile_GovernmentIdNumber");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ApplicantProfile",
                table: "ApplicantProfile",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ApplicantProfile_Users_UserId",
                table: "ApplicantProfile",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PropertyApplications_ApplicantProfile_ApplicantId",
                table: "PropertyApplications",
                column: "ApplicantId",
                principalTable: "ApplicantProfile",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
