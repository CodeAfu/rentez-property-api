using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RentEZApi.Migrations
{
    /// <inheritdoc />
    public partial class AddApplicantProfile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PropertyApplications_Users_ApplicantId",
                table: "PropertyApplications");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Property",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<int>(
                name: "Income",
                table: "Property",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MaxLeaseTerm",
                table: "Property",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MaxOccupants",
                table: "Property",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MinLeaseTerm",
                table: "Property",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "PetsAllowed",
                table: "Property",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "SmokingAllowed",
                table: "Property",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "ApplicantProfile",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    MonthlyIncome = table.Column<decimal>(type: "numeric", nullable: true),
                    EmployerName = table.Column<string>(type: "text", nullable: true),
                    GovernmentIdType = table.Column<string>(type: "text", nullable: true),
                    GovernmentIdNumber = table.Column<string>(type: "text", nullable: true),
                    NumberOfOccupants = table.Column<int>(type: "integer", nullable: true),
                    HasPets = table.Column<bool>(type: "boolean", nullable: true),
                    PetDetails = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicantProfile", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApplicantProfile_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApplicantProfile_GovernmentIdNumber",
                table: "ApplicantProfile",
                column: "GovernmentIdNumber");

            migrationBuilder.CreateIndex(
                name: "IX_ApplicantProfile_UserId",
                table: "ApplicantProfile",
                column: "UserId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_PropertyApplications_ApplicantProfile_ApplicantId",
                table: "PropertyApplications",
                column: "ApplicantId",
                principalTable: "ApplicantProfile",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PropertyApplications_ApplicantProfile_ApplicantId",
                table: "PropertyApplications");

            migrationBuilder.DropTable(
                name: "ApplicantProfile");

            migrationBuilder.DropColumn(
                name: "Income",
                table: "Property");

            migrationBuilder.DropColumn(
                name: "MaxLeaseTerm",
                table: "Property");

            migrationBuilder.DropColumn(
                name: "MaxOccupants",
                table: "Property");

            migrationBuilder.DropColumn(
                name: "MinLeaseTerm",
                table: "Property");

            migrationBuilder.DropColumn(
                name: "PetsAllowed",
                table: "Property");

            migrationBuilder.DropColumn(
                name: "SmokingAllowed",
                table: "Property");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Property",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_PropertyApplications_Users_ApplicantId",
                table: "PropertyApplications",
                column: "ApplicantId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
