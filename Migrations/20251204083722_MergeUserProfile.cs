using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RentEZApi.Migrations
{
    /// <inheritdoc />
    public partial class MergeUserProfile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PropertyApplications_ApplicantProfiles_ApplicantProfileId",
                table: "PropertyApplications");

            migrationBuilder.DropTable(
                name: "ApplicantProfiles");

            migrationBuilder.RenameColumn(
                name: "ApplicantProfileId",
                table: "PropertyApplications",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_PropertyApplications_ApplicantProfileId",
                table: "PropertyApplications",
                newName: "IX_PropertyApplications_UserId");

            migrationBuilder.AddColumn<string>(
                name: "EmployerName",
                table: "Users",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GovernmentIdNumber",
                table: "Users",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GovernmentIdType",
                table: "Users",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "HasPets",
                table: "Users",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "MonthlyIncome",
                table: "Users",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NumberOfOccupants",
                table: "Users",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PetDetails",
                table: "Users",
                type: "text",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_PropertyApplications_Users_UserId",
                table: "PropertyApplications",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PropertyApplications_Users_UserId",
                table: "PropertyApplications");

            migrationBuilder.DropColumn(
                name: "EmployerName",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "GovernmentIdNumber",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "GovernmentIdType",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "HasPets",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "MonthlyIncome",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "NumberOfOccupants",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "PetDetails",
                table: "Users");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "PropertyApplications",
                newName: "ApplicantProfileId");

            migrationBuilder.RenameIndex(
                name: "IX_PropertyApplications_UserId",
                table: "PropertyApplications",
                newName: "IX_PropertyApplications_ApplicantProfileId");

            migrationBuilder.CreateTable(
                name: "ApplicantProfiles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    EmployerName = table.Column<string>(type: "text", nullable: true),
                    GovernmentIdNumber = table.Column<string>(type: "text", nullable: true),
                    GovernmentIdType = table.Column<string>(type: "text", nullable: true),
                    HasPets = table.Column<bool>(type: "boolean", nullable: true),
                    MonthlyIncome = table.Column<decimal>(type: "numeric", nullable: true),
                    NumberOfOccupants = table.Column<int>(type: "integer", nullable: true),
                    PetDetails = table.Column<string>(type: "text", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicantProfiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApplicantProfiles_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApplicantProfiles_GovernmentIdNumber",
                table: "ApplicantProfiles",
                column: "GovernmentIdNumber");

            migrationBuilder.CreateIndex(
                name: "IX_ApplicantProfiles_UserId",
                table: "ApplicantProfiles",
                column: "UserId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_PropertyApplications_ApplicantProfiles_ApplicantProfileId",
                table: "PropertyApplications",
                column: "ApplicantProfileId",
                principalTable: "ApplicantProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
