using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace personwatcherapi.Data.Migrations
{
    /// <inheritdoc />
    public partial class EventPredictability_Added : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EventPredictability",
                table: "Persons",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EventPredictability",
                table: "Persons");
        }
    }
}
