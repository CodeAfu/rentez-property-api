using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RentEZApi.Migrations
{
    /// <inheritdoc />
    public partial class UpdatePropertyTableStructure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Property",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Address = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    City = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    State = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Images = table.Column<string[]>(type: "text[]", nullable: false),
                    DepositRequired = table.Column<bool>(type: "boolean", nullable: true),
                    BillsIncluded_Wifi = table.Column<bool>(type: "boolean", nullable: true),
                    BillsIncluded_Electricity = table.Column<bool>(type: "boolean", nullable: true),
                    BillsIncluded_Water = table.Column<bool>(type: "boolean", nullable: true),
                    BillsIncluded_Gas = table.Column<bool>(type: "boolean", nullable: true),
                    RoomType = table.Column<string[]>(type: "text[]", nullable: false),
                    PreferredRaces = table.Column<string[]>(type: "text[]", nullable: false),
                    PreferredOccupation = table.Column<string[]>(type: "text[]", nullable: false),
                    LeaseTermCategory = table.Column<string[]>(type: "text[]", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Property", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Property");
        }
    }
}
