using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace personwatcherapi.Data.Migrations
{
    /// <inheritdoc />
    public partial class Mercury_UranusPos_Added : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MercuryPos",
                table: "Persons",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UranusPos",
                table: "Persons",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MercuryPos",
                table: "Persons");

            migrationBuilder.DropColumn(
                name: "UranusPos",
                table: "Persons");
        }
    }
}
